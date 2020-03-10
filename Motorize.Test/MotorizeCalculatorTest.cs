using Microsoft.VisualStudio.TestTools.UnitTesting;
using Motorize.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Motorize.Test
{
  [TestClass]
  public class MotorizeCalculatorTest
  {
    private const float delta = 0.1F;

    //1: Calculate Mk (Couple au regime nominal)
    [TestMethod]
    public void Calculate_CoupleRegimeNominal()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();

      var regime = calculator.CalculateCoupleRegimeNominal(cvvm.Data.Nmax, cvvm.Data.nN);

      this.AssertAccurateEnough(regime, 166.6M);
    }

    //2: Calculate Mr (Reserve du couple)
    [TestMethod]
    public void Calculate_ReserveCouple()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();

      var Mkn = calculator.CalculateCoupleRegimeNominal(cvvm.Data.Nmax, cvvm.Data.nN);
      var Mr = calculator.CalculateReserveCouple(Mkn, cvvm.Data.Mkmax);

      this.AssertAccurateEnough(Mr, 6.8M);
    }
    //3: Calculate k-omega (Coefficient d'adaptabilité)
    [TestMethod]
    public void Calculate_CoefficientAdaptabilite()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();

      var ko = calculator.CalculateCoefficientAdaptabiliteRegime(cvvm.Data.nN, cvvm.Data.nMkmax);

      this.AssertAccurateEnough(ko, 1.5M, 0.3F);
    }

    //4: Calculate km (Coefficient d'adaptabilité par rapport au couple)
    [TestMethod]
    public void Calculate_CoefficientAdaptabiliteCouple()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();

      var Mkn = calculator.CalculateCoupleRegimeNominal(cvvm.Data.Nmax, cvvm.Data.nN);
      var km = calculator.CalculateCoefficientAdaptabiliteCouple(Mkn, cvvm.Data.Mkmax);
      this.AssertAccurateEnough(km, 1.06M);
    }
    //5: Calculate Hg (Centre de masse)
    [TestMethod]
    public void Calculate_CentreMasse()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();

      var Hg = calculator.CalculateCentreMasse(cvvm.Data.H);
      this.AssertAccurateEnough(Hg, 0.569M);
    }
    //6: Calculate Omega-v (Coefficient de resistance total)
    [TestMethod]
    public void Calculate_CoefficientResistanceTotal()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();

      var ov = calculator.CalculateCoefficientResistanceTotalDeplacement(calculator.GetCoefficientResistanceRoulement(), cvvm.Data.α);
      this.AssertAccurateEnough(ov, 0.02M);
    }
    //7: Calculate F (Surface Frontal)
    [TestMethod]
    public void Calculate_SurfaceFrontale()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();

      var F = calculator.CalculateSurfaceFrontale(cvvm.Data.B, cvvm.Data.H);
      this.AssertAccurateEnough(F, 1.94M);
    }

    //8: Generate dataset for Ne and draw graph for user to determine nv
    [TestMethod]
    public void GenerateDataSetForNe()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();
      decimal[] valuesOfN = new decimal[] { 500, 1300, 2100, 2900, 2700, 4400, 5200, 5400 };

      //normally An, Bn and Cn are calculated, but for clients convenience they will be static

      var dataSet = calculator.GetDatasetCourbePuissanceMoteur(cvvm.Data.Nmax, cvvm.Data.nN, cvvm.EmpiricCoefficient.An, cvvm.EmpiricCoefficient.Bn, cvvm.EmpiricCoefficient.Cn, valuesOfN);

      Assert.AreEqual(dataSet[0].Item1, 500);
      this.AssertAccurateEnough(dataSet[0].Item2, 5.801145214M);
      Assert.AreEqual(dataSet[dataSet.Count - 2].Item1, 5200);
      this.AssertAccurateEnough(dataSet[dataSet.Count - 1].Item2, 94.2M);

    }

    //9: Calculate ibv1 (rapport 1er vitesse)
    [TestMethod]
    public void CalculateRapportPremiereVitesse()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();
      var ibvc = 1;
      var nv = 5750;

      var ip = calculator.CalculateRapportPont(cvvm.Data.Rk, nv, cvvm.Data.Vmax, ibvc);
      var first = calculator.CalculateRapportPremiereVitesse(cvvm.Data.mo * 9.81M, cvvm.Data.Ψmax, cvvm.Data.Rk, cvvm.Data.Mkmax, ibvc, ip, cvvm.Data.nt, calculator.GetCoefficientCorrection());


      this.AssertAccurateEnough(first, 2.68M);
    }

    //10: Calculate ibvi (rapport autre vitesse)
    [TestMethod]
    public void CalculateRapportAutreVitesse()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();
      var ibvc = 1;
      var nv = 5750;
      decimal first = this.CalculateRapport1erVitesse(calculator, cvvm, ibvc, nv);

      decimal second = calculator.CalculateRapportAutreVitesse(6, 2, first);
      decimal third = calculator.CalculateRapportAutreVitesse(6, 3, first);
      decimal fourth = calculator.CalculateRapportAutreVitesse(6, 4, first);
      decimal fifth = calculator.CalculateRapportAutreVitesse(6, 5, first);
      decimal sixth = calculator.CalculateRapportAutreVitesse(6, 6, first);

      this.AssertAccurateEnough(second, 2.2M);
      this.AssertAccurateEnough(third, 1.8M);
      this.AssertAccurateEnough(fourth, 1.48M);
      this.AssertAccurateEnough(fifth, 1.2M);
      this.AssertAccurateEnough(sixth, 1M);
    }

    //11: Calculate iti
    //[TestMethod]
    public void CalculateRapportGlobauxTransmission()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();
      var ibvc = 1;
      var nv = 5750;

      decimal[] iti = this.GetIti(calculator, cvvm, ibvc, nv);

    }

    //12: Generate matrix for Nei
    [TestMethod]
    public void GenerateMatrixForNei()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();
      decimal[][] velocities = this.GetVelocities();
      var ibvc = 1;
      var nv = 5750;


      decimal[] iti = this.GetIti(calculator, cvvm, ibvc, nv);
      List<Tuple<decimal, decimal>>[] matrix = GetNei(calculator, cvvm, velocities, iti);

      Assert.AreEqual(matrix[0][0].Item1, 2);
      this.AssertAccurateEnough(matrix[0][0].Item2, 534.573M, 0.5F);
      Assert.AreEqual(matrix[4][6].Item1, 33);
      this.AssertAccurateEnough(matrix[4][6].Item2, 3949.910M);
      Assert.AreEqual(matrix[5][6].Item1, 35);
      this.AssertAccurateEnough(matrix[5][6].Item2, 3491.082M);

    }

    private static List<Tuple<decimal, decimal>>[] GetNei(Calculator calculator, ChartValuesViewModel cvvm, decimal[][] velocities, decimal[] iti)
    {

      //NEI
      List<Tuple<decimal, decimal>>[] matrix = new List<Tuple<decimal, decimal>>[velocities.Length];

      for (int i = 0; i < velocities.Length; i++)
      {
        matrix[i] = calculator.GetDatasetRegimeMoteurChaqueRapport(iti[i], cvvm.Data.Rk, velocities[i]);
      }

      return matrix;
    }


    //13: Generate dataset for Mk 
    [TestMethod]
    public void GenerateDataSetForMk()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();
      decimal[] valuesOfN = new decimal[] { 500, 1300, 2100, 2900, 2700, 4400, 5200, 5400 };

      decimal[][] velocities = this.GetVelocities();
      var ibvc = 1;
      var nv = 5750;

      List<Tuple<decimal, decimal>>[] matrix = GetMk(calculator, cvvm, velocities, ibvc, nv);
    }

    private List<Tuple<decimal, decimal>>[] GetMk(Calculator calculator, ChartValuesViewModel cvvm, decimal[][] velocities, int ibvc, int nv)
    {
      decimal[] iti = this.GetIti(calculator, cvvm, ibvc, nv);
      List<Tuple<decimal, decimal>>[] nei = GetNei(calculator, cvvm, velocities, iti);

      //MK
      List<Tuple<decimal, decimal>>[] matrix = new List<Tuple<decimal, decimal>>[velocities.Length];

      for (int i = 0; i < velocities.Length; i++)
      {
        matrix[i] = calculator.GetDatasetCourbeCoupleMoteur(cvvm.Data.Mkmax, cvvm.Data.nN, cvvm.EmpiricCoefficient.An, cvvm.EmpiricCoefficient.Bn, cvvm.EmpiricCoefficient.Cn, nei[i]);
      }

      return matrix;
    }


    //14: Generate matrix for PT
    [TestMethod]
    public void GenerateMatrixForPT()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();
      var ibvc = 1;
      var nv = 5750;
      decimal[] valuesOfN = new decimal[] { 500, 1300, 2100, 2900, 2700, 4400, 5200, 5400 };
      List<Tuple<decimal, decimal>>[] matrix = GetPt(calculator, cvvm, ibvc, nv, valuesOfN);

      Assert.AreEqual(matrix[0][0].Item1, 2);
      this.AssertAccurateEnough(matrix[0][0].Item2, 534.573M, 0.5F);
      Assert.AreEqual(matrix[4][6].Item1, 33);
      this.AssertAccurateEnough(matrix[4][6].Item2, 3949.910M);
      Assert.AreEqual(matrix[5][6].Item1, 35);
      this.AssertAccurateEnough(matrix[5][6].Item2, 3491.082M);

    }

    private List<Tuple<decimal, decimal>>[] GetPt(Calculator calculator, ChartValuesViewModel cvvm, int ibvc, int nv, decimal[] valuesOfN)
    {
      // TODO: Come back here to verify MK; From document Mk needs to change according to velocity
      // and rapport de vitesse but it is not changing thus values cannot be used effectively on formula

      var Mki = GetMk(calculator, cvvm, this.GetVelocities(), ibvc, nv);

      decimal[] iti = this.GetIti(calculator, cvvm, ibvc, nv);

      //Mki
      List<Tuple<decimal, decimal>>[] matrix = new List<Tuple<decimal, decimal>>[Mki.Length];

      for (int i = 0; i < Mki.Length; i++)
      {
        matrix[i] = calculator.GetDatasetForceTractionMoteur(iti[i], cvvm.Data.Rk, cvvm.Data.nt, Mki[i]);
      }

      return matrix;
    }


    //15: Generate matrix for Pw
    [TestMethod]
    public void GenerateMatrixForPw()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();
      decimal[][] velocities = this.GetVelocities();

      //Pw
      var matrix = GetPw(calculator, cvvm, velocities);


      // TODO: Assert

    }

    private List<Tuple<decimal, decimal>>[] GetPw(Calculator calculator, ChartValuesViewModel cvvm, decimal[][] velocities)
    {
      List<Tuple<decimal, decimal>>[] matrix = new List<Tuple<decimal, decimal>>[velocities.Length];

      for (int i = 0; i < velocities.Length; i++)
      {
        matrix[i] = calculator.GetDatasetForceAerodynamiqueResistance(cvvm.Data.H, cvvm.Data.B, cvvm.Data.Cf, cvvm.Data.Kv, velocities[i]);
      }
      return matrix;
    }

    //15: Generate matrix for D
    [TestMethod]
    public void GenerateMatrixForD()
    {
      Calculator calculator = new Calculator();
      ChartValuesViewModel cvvm = this.GetVM();
      decimal[][] velocities = this.GetVelocities();
      var ibvc = 1;
      var nv = 5750;
      decimal[] valuesOfN = new decimal[] { 500, 1300, 2100, 2900, 2700, 4400, 5200, 5400 };
      List<Tuple<decimal, decimal>>[] matrix = new List<Tuple<decimal, decimal>>[velocities.Length];


      List<Tuple<decimal, decimal>>[] Pt = GetPt(calculator, cvvm, ibvc, nv, valuesOfN);
      List<Tuple<decimal, decimal>>[] Pw = GetPw(calculator, cvvm, velocities);

      for (int i = 0; i < velocities.Length; i++)
      {
        matrix[i] = calculator.GetDatasetFacteurDynamique(Pt[i], Pw[i], cvvm.Data.mo);
      }


      // TODO: Assert

    }

    private decimal[][] GetVelocities()
    {
      return new decimal[][]{
        new decimal[] { 2, 3, 4, 5, 6, 7, 8 },
        new decimal[] { 4, 6, 8, 10, 12, 14, 16 },
        new decimal[] { 8, 12, 16, 20, 24, 28, 30 },
        new decimal[] { 10, 14, 17, 19, 26, 29, 32 },
        new decimal[] { 12, 15, 19, 22, 27, 30, 33 },
        new decimal[] { 15, 17, 21, 24, 28, 31, 35 }
      };
    }

    private decimal[] GetIti(Calculator calculator, ChartValuesViewModel cvvm, int ibvc, int nv)
    {
      var ip = calculator.CalculateRapportPont(cvvm.Data.Rk, nv, cvvm.Data.Vmax, ibvc);
      decimal first = this.CalculateRapport1erVitesse(calculator, cvvm, ibvc, nv);

      decimal second = calculator.CalculateRapportAutreVitesse(6, 2, first);
      decimal third = calculator.CalculateRapportAutreVitesse(6, 3, first);
      decimal fourth = calculator.CalculateRapportAutreVitesse(6, 4, first);
      decimal fifth = calculator.CalculateRapportAutreVitesse(6, 5, first);
      decimal sixth = calculator.CalculateRapportAutreVitesse(6, 6, first);
      decimal it1 = calculator.CalculateRapportGlobauxTransmission(first, ip);
      decimal it2 = calculator.CalculateRapportGlobauxTransmission(second, ip);
      decimal it3 = calculator.CalculateRapportGlobauxTransmission(third, ip);
      decimal it4 = calculator.CalculateRapportGlobauxTransmission(fourth, ip);
      decimal it5 = calculator.CalculateRapportGlobauxTransmission(fifth, ip);
      decimal it6 = calculator.CalculateRapportGlobauxTransmission(sixth, ip);

      return new decimal[] { it1, it2, it3, it4, it5, it6 };
    }



    private decimal CalculateRapport1erVitesse(Calculator calculator, ChartValuesViewModel cvvm, int ibvc, int nv)
    {
      var ip = calculator.CalculateRapportPont(cvvm.Data.Rk, nv, cvvm.Data.Vmax, ibvc);
      var first = calculator.CalculateRapportPremiereVitesse(cvvm.Data.mo * 9.81M, cvvm.Data.Ψmax, cvvm.Data.Rk, cvvm.Data.Mkmax, ibvc, ip, cvvm.Data.nt, calculator.GetCoefficientCorrection());
      return first;
    }

    private ChartValuesViewModel GetVM() => new ChartValuesViewModel
    {
      Data = new DataViewModel
      {
        mo = 1240,
        B = 1.42M,
        H = 1.709M,
        Mkmax = 178,
        nMkmax = 4400,
        nN = 5400,
        Kv = 0.280M,
        Rk = 0.25M,
        Nmax = 94.2M,
        Vmax = 57.77M,
        nt = 0.96M,
        Cf = 0.8M,
        Ψmax = 0.35M
      },
      EmpiricCoefficient = new EmpiricCoefficient
      {
        An = 0.53M,
        Bn = 1.56M,
        Cn = 1.09M
      }
    };

    private void AssertAccurateEnough(decimal val1, decimal val2, float delta = delta)
    {
      Assert.AreEqual(Convert.ToSingle(val1), Convert.ToSingle(val2), delta: (delta));
    }
  }
}