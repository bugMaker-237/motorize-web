namespace Motorize {
  using System.Collections.Generic;
  using System;
  using Motorize.Models;
  public class Calculator {
    //Coefficient resistance roulement
    private const decimal f = 0.015M;
    //Coefficient de correction kp
    private const decimal kp = 0.89M;
    // Coefficient de resistance Maximal
    private const decimal Ψmax = 0.35M;

    //I think this will change (Rapport de boite de vitesse complementaire)
    private const decimal ibvc = 1;

    //I think this will change (Rapport superieur de boite de vitesse)
    private const decimal ibvs = 1;
    public void Calculate (ChartValuesViewModel viewModel) {
      decimal coupleNominal = CalculateCoupleRegimeNominal (
        viewModel.Data.Nmax,
        viewModel.Data.nN
      );

      decimal reserveCouple = CalculateReserveCouple (
        coupleNominal,
        viewModel.Data.Nmax
      );

      decimal coefficientAdaptabiliteRegime = CalculateCoefficientAdaptabiliteRegime (
        viewModel.Data.nN,
        viewModel.Data.nMkmax
      );

      decimal coefficientAdaptabiliteCouple = CalculateCoefficientAdaptabiliteCouple (
        coupleNominal,
        viewModel.Data.Mkmax
      );
      decimal centreMasse = CalculateCentreMasse (
        viewModel.Data.H
      );
      // decimal coefficientResistanceRoulement = CalculateCoefficientResistanceRoulement ();
      // decimal coefficientResistanceTotalDeplacement = CalculateCoefficientResistanceTotalDeplacement (
      //   CalculateCoefficientResistanceRoulement (),
      //   viewModel.Data.PenteInclinason
      // );
      decimal coefficientCorrection = GetCoefficientCorrection ();

      // decimal surfaceFrontale = CalculateSurfaceFrontale (
      //   viewModel.Data.VoieAuSol,
      //   viewModel.Data.Hauteur
      // );
      // List<Tuple<decimal, decimal>> puissanceEffectiveDataset =
      //   GetDatasetPuissanceEffectiveVelocite (
      //     viewModel.Data.MasseVehicule,
      //     CalculateCoefficientResistanceTotalDeplacement (
      //       GetCoefficientResistanceRoulement (),
      //       viewModel.Data.PenteInclinason
      //     ),
      //     viewModel.Data.CoefficientAerodynamic,
      //     CalculateSurfaceFrontale (
      //       viewModel.Data.VoieAuSol,
      //       viewModel.Data.Hauteur
      //     ),
      //     viewModel.Data.RendementGlobal,
      //     GetCoefficientCorrection ()
      //   );
      // decimal rapportSuperieurTransmission = CalculateR
      // decimal rapportTransmission = CalculateRapportTransmission ();
    }

    /// <summary>
    /// Generates Dataset for Nev versus Vmax graph.
    /// </summary>
    /// <param name="dataViewModel">Data viewmodel</param>
    /// <returns>The dataset</returns>
    public List<Tuple<decimal, decimal>> GenerateDatasetForNevVmax (
        DataViewModel dataViewModel,
        decimal[] velocities
      ) =>
      GetDatasetPuissanceEffectiveVelocite (
        CalculateGravitationalForce (dataViewModel.mo),
        CalculateCoefficientResistanceTotalDeplacement (
          GetCoefficientResistanceRoulement (),
          dataViewModel.α
        ),
        dataViewModel.Kv,
        CalculateSurfaceFrontale (
          dataViewModel.B,
          dataViewModel.H
        ),
        dataViewModel.nt,
        GetCoefficientCorrection (),
        velocities
      );

    private decimal CalculateGravitationalForce (decimal Mo) => 9.81M * Mo;

    ///<Summary>
    ///Calculates Nev Dataset
    ///</Summary>
    private List<Tuple<decimal, decimal>> GetDatasetPuissanceEffectiveVelocite (
      decimal Ga,
      decimal Ψv,
      decimal kv,
      decimal F,
      decimal nt,
      decimal kp,
      decimal[] velocities
    ) {
      // decimal[] velocities = new decimal[] { 0, 10, 20, 30, 40, 50, 57.77M, 60 };

      List<Tuple<decimal, decimal>> tableOfValues = new List<Tuple<decimal, decimal>> ();

      for (int i = 0; i < velocities.Length; i++) {
        tableOfValues.Add (new Tuple<decimal, decimal> (velocities[i], CalculatePuissanceEffective (velocities[i], Ga, Ψv, kv, F, nt, kp)));
      }
      return tableOfValues;
    }

    ///<Summary>
    ///Calculates Dataset of Nei
    ///<returns> Returns Velocity against Nei</returns>
    ///</Summary>
    public List<Tuple<decimal, decimal>> GetDatasetRegimeMoteurChaqueRapport (
      decimal iti,
      decimal Rk,
      decimal[] velocities
    ) {

      List<Tuple<decimal, decimal>> tableOfValues = new List<Tuple<decimal, decimal>> ();

      for (int i = 0; i < velocities.Length; i++) {
        tableOfValues.Add (new Tuple<decimal, decimal> (velocities[i], CalculateRegimeMoteurChaqueRapport (iti, Rk, velocities[i])));
      }
      return tableOfValues;
    }

    /// <summary>
    /// Calculates Ne Dataset
    /// </summary>
    public List<Tuple<decimal, decimal>> GetDatasetCourbePuissanceMoteur (
      decimal Nmax,
      decimal nN,
      decimal An,
      decimal Bn,
      decimal Cn,
      decimal[] n
    ) {

      List<Tuple<decimal, decimal>> tableOfValues = new List<Tuple<decimal, decimal>> ();

      for (int i = 0; i < n.Length; i++) {
        tableOfValues.Add (new Tuple<decimal, decimal> (n[i],
          CalculateCourbePuissanceMoteur (Nmax, nN, An, Bn, Cn, n[i])));
      }
      return tableOfValues;
    }

    /// <summary>
    /// Calculates Pt Dataset
    /// </summary>
    public List<Tuple<decimal, decimal>> GetDatasetForceTractionMoteur (
      decimal iti,
      decimal Rk,
      decimal nt,
      List<Tuple<decimal, decimal>> Mk
    ) {

      List<Tuple<decimal, decimal>> tableOfValues = new List<Tuple<decimal, decimal>> ();

      for (int i = 0; i < Mk.Count; i++) {
        tableOfValues.Add (new Tuple<decimal, decimal> (Mk[i].Item2,
          CalculateForceTractionMoteur (iti, Rk, nt, Mk[i].Item2)));
      }
      return tableOfValues;
    }

    private decimal CalculateForceTractionMoteur (
      decimal iti,
      decimal Rk,
      decimal nt,
      decimal Mk
    ) => (nt * Mk * iti) / (1000 * Rk);

    /// <summary>
    /// Calculates Pω Dataset
    /// </summary>
    public List<Tuple<decimal, decimal>> GetDatasetForceAerodynamiqueResistance (
      decimal H,
      decimal B,
      decimal Cf,
      decimal Kv,
      decimal[] V
    ) {

      List<Tuple<decimal, decimal>> tableOfValues = new List<Tuple<decimal, decimal>> ();
      // un coefficient de surface aerodynamique
      decimal F = 0.86M * B * H;
      for (int i = 0; i < V.Length; i++) {
        tableOfValues.Add (new Tuple<decimal, decimal> (V[i],
          CalculateForceAerodynamiqueResistance (Cf, Kv, F, V[i])));
      }
      return tableOfValues;
    }

    private decimal CalculateForceAerodynamiqueResistance (
      decimal Cf,
      decimal Kv,
      decimal F,
      decimal V
    ) => (Cf * Kv * F * Pow (V, 2)) / 1000;

    /// <summary>
    /// Calculates D Dataset
    /// </summary>
    public List<Tuple<decimal, decimal>> GetDatasetFacteurDynamique (
      List<Tuple<decimal, decimal>> Pt,
      List<Tuple<decimal, decimal>> Pω,
      decimal mo
    ) {
      List<Tuple<decimal, decimal>> tableOfValues = new List<Tuple<decimal, decimal>>();
      decimal mass = 10M * mo;
      for (int i = 0; i < Pt.Count; i++)
      {
        tableOfValues.Add(new Tuple<decimal, decimal>(Pω[i].Item1,
                CalculateValueD(Pt[i].Item1, Pω[i].Item1, mass)));
      }
      return tableOfValues;
    }

    private decimal CalculateValueD(decimal Pt, decimal Pω, decimal mass)
    {
      return ((Pt - Pω) / mass) * 1000M;
    }

    /// <summary>
    /// Calculates Mk Dataset
    /// </summary>
    public List<Tuple<decimal, decimal>> GetDatasetCourbeCoupleMoteur (
      decimal Mkmax,
      decimal nN,
      decimal An,
      decimal Bn,
      decimal Cn,
      List<Tuple<decimal, decimal>> ne
    ) {

      List<Tuple<decimal, decimal>> tableOfValues = new List<Tuple<decimal, decimal>> ();

      for (int i = 0; i < ne.Count; i++) {
        tableOfValues.Add (new Tuple<decimal, decimal> (ne[i].Item1,
          CalculateCourbeCoupleMoteur (Mkmax, nN, An, Bn, Cn, ne[i].Item2)));
      }
      return tableOfValues;
    }

    /// <summary>
    /// Calculates Mk
    /// </summary>
    public decimal CalculateCourbeCoupleMoteur (
      decimal Mkmax,
      decimal nN,
      decimal An,
      decimal Bn,
      decimal Cn,
      decimal nei
    ) {
      decimal value = (nei / nN);
      return Mkmax * (An + (Bn * value) - (Cn * Pow (value, 2)));
    }

    /// <summary>
    /// Calculates Ne
    /// </summary>
    public decimal CalculateCourbePuissanceMoteur (
      decimal Nmax,
      decimal nN,
      decimal An,
      decimal Bn,
      decimal Cn,
      decimal n
    ) {
      decimal value = (n / nN);
      return Nmax * ((An * value) + (Bn * Pow (value, 2)) - (Cn * Pow (value, 3)));
    }

    private decimal CalculateRegimeMoteurChaqueRapport (
      decimal iti,
      decimal Rk,
      decimal velocity
    ) => (30 * velocity * iti) / (3.14M * Rk);

    private decimal CalculatePuissanceEffective (
      decimal v,
      decimal g,
      decimal Ψv,
      decimal kv,
      decimal F,
      decimal nt,
      decimal kp
    ) => ((g * Ψv + kv * F * v * v) / (1000 * nt * kp)) * v;

    /// <summary>
    /// Calculates Ip
    /// </summary>
    public decimal CalculateRapportPont (
      decimal Rk,
      decimal nv,
      decimal Vmax,
      decimal ibvc
    ) => (0.105M * Rk * nv) / (ibvc * Vmax);

    /// <summary>
    /// Calculates ibv1
    /// </summary>
    public decimal CalculateRapportPremiereVitesse (
      decimal Ga,
      decimal Ψmax,
      decimal Rk,
      decimal Mkmax,
      decimal ibvc,
      decimal ip,
      decimal nt,
      decimal kv
    ) => (Ga * Ψmax * Rk) / (Mkmax * ibvc * ip * nt * kv);

    /// <summary>
    /// Calculates ibv(i) where m is the velocityNumber
    /// </summary>
    public decimal CalculateRapportAutreVitesse (
      int numberofVelocities,
      int velocityNumber,
      decimal firstVelocityValue
    ) {
      
      return Pow (
        Pow (
          firstVelocityValue,
          numberofVelocities - velocityNumber
        ) *
        Pow (
          GetRapportSuperieurBoiteVitesse (),
          velocityNumber - 1
        ),
        1M / (numberofVelocities - 1)
      );
    }

    /// <summary>
    ///  Calculate iti
    /// </summary>
    public decimal CalculateRapportGlobauxTransmission (
      decimal ibvi,
      decimal ip
    ) => ibvi * ip;

    ///<Summary>
    /// Calculates An
    ///</Summary>
    public decimal CalculateCoefficientEmpiricA (decimal Mr) => 1 - (25 / Mr);

    ///<Summary>
    /// Calculates Bn
    ///</Summary>
    public decimal CalculateCoefficientEmpiricB (decimal Mr) => (50 / Mr - 1);

    ///<Summary>
    /// Calculates Cn
    ///</Summary>
    public decimal CalculateCoefficientEmpiricC (decimal Mr) => (25 / Mr);

    ///<Summary>
    /// Calculates F
    ///</Summary>
    public decimal CalculateSurfaceFrontale (decimal B, decimal H) => 0.8M * B * H;

    ///<Summary>
    ///Calculates MkN
    ///</Summary>
    public decimal CalculateCoupleRegimeNominal (decimal Nmax, decimal nN) => 9554 * (Nmax / nN);

    ///<Summary>
    ///Calculates Mr
    ///</Summary>
    public decimal CalculateReserveCouple (decimal Mkn, decimal Mkmax) => ((Mkmax / Mkn) - 1) * 100;

    ///<Summary>
    /// Calculates KΩ
    ///</Summary>
    public decimal CalculateCoefficientAdaptabiliteRegime (decimal nN, decimal nMkmax) => nN / nMkmax;

    ///<Summary>
    ///Test if CoefficientAddaptabilite is Correct
    ///</Summary>
    private bool isCoefficientAddaptabiliteCorrect (decimal result) => result <= 2.5M || result >= 1.5M;

    ///<Summary>
    /// Calculates Km
    ///</Summary>
    public decimal CalculateCoefficientAdaptabiliteCouple (decimal Mkn, decimal Mkmax) => Mkmax / Mkn;

    ///<Summary>
    /// Calculates Hg
    ///</Summary>
    public decimal CalculateCentreMasse (decimal H) => H / 3;

    ///<Summary>
    /// Returns constant f
    ///</Summary>
    public decimal GetCoefficientResistanceRoulement () => f;

    ///<Summary>
    /// Calculates Ψv
    ///</Summary>
    public decimal CalculateCoefficientResistanceTotalDeplacement (decimal f, double alpha) =>
      f * Convert.ToDecimal (Math.Cos (alpha)) + Convert.ToDecimal (Math.Sin (alpha));

    ///<Summary>
    /// Returns constante kp
    ///</Summary>
    public decimal GetCoefficientCorrection () => kp;

    ///<Summary>
    /// Calculates Ψmax
    ///</Summary>
    private decimal CalculateCoefficientResistanceMax () => Ψmax;

    ///<Summary>
    /// Returns constante ibvc
    ///</Summary>
    public decimal GetRapportVitesseComplementaire () => ibvc;

    /// <summary>
    /// Returns constante ibs
    /// </summary>
    public decimal GetRapportSuperieurBoiteVitesse () => ibvs;

    private decimal Pow (decimal number, decimal to) => Convert.ToDecimal (
      Math.Pow (Convert.ToDouble (number), Convert.ToDouble (to))
    );

  }
}