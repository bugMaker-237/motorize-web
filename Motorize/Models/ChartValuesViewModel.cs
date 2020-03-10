using System.ComponentModel.DataAnnotations;

namespace Motorize.Models
{
  public class ChartValuesViewModel
  {
    public DataViewModel Data { get; set; } = new DataViewModel();
    public VehicleXticsViewModel VehicleXtics { get; set; } = new VehicleXticsViewModel();
    public EmpiricCoefficient EmpiricCoefficient { get; set; } = new EmpiricCoefficient();
  }
  public class DataViewModel
  {
    private const string DEFAULT_REQUIRE_MSG = "Champs requis";
    private const string DEFAULT_0_MSG = "La valeur doit être supérieure à 0";
    private const double DEFAULT_VAL = 0.0001;
    ///<Summary>
    /// VoieAuSol
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal B { get; set; }
    ///<Summary>
    /// Hauteur
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal H { get; set; }
    ///<Summary>
    /// RayonCinematique
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal Rk { get; set; }
    ///<Summary>
    /// CoefficientAerodynamic
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal Kv { get; set; }
    ///<Summary>
    /// MasseVehicule
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal mo { get; set; }
    ///<Summary>
    /// RendementGlobal
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, 1, ErrorMessage = DEFAULT_0_MSG)]
    public decimal nt { get; set; }
    ///<Summary>
    /// CoefficientSurface
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(0.6, 0.8, ErrorMessage = "Cf doit être compris entre 0.6 et 0.8")]
    public decimal Cf { get; set; }
    ///<Summary>
    /// CoupleMaximal
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal Mkmax { get; set; }
    ///<Summary>
    /// PuissanceMaximal
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal Nmax { get; set; }
    ///<Summary>
    /// RegimePuissanceMaximal
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal nN { get; set; }
    ///<Summary>
    /// VitesseMaximal
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal Vmax { get; set; }
    ///<Summary>
    ///RegimeNominalCoupleMaximal
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal nMkmax { get; set; }
    ///<Summary>
    ///PenteInclinason
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public double α { get; set; }
    ///<Summary>
    ///CoefficientResistanceMaximal
    ///</Summary>
    [Required(ErrorMessage = DEFAULT_REQUIRE_MSG)]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal Ψmax { get; set; }
  }
  public class VehicleXticsViewModel
  {
    [Required]
    [Range(0, 1, ErrorMessage = "Sélectionner un type")]
    public int TypeVehicule { get; set; } = -1;
    [Required]
    [Range(0, 1, ErrorMessage = "Sélectionner un type")]
    public int TypeMoteur { get; set; } = -1;
    [Required]
    [Range(0, 1, ErrorMessage = "Sélectionner un type")]
    public int TypeTransmission { get; set; } = -1;

    [Required(ErrorMessage = "Entrer le nom du vehicule")]
    public string Vehicle { get; set; }
  }

  public class EmpiricCoefficient
  {
    private const string DEFAULT_0_MSG = "La valeur doit être supérieure à 0";
    private const double DEFAULT_VAL = 0.0001;
    [Required]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal An { get; set; }
    [Required]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal Bn { get; set; }
    [Required]
    [Range(DEFAULT_VAL, double.MaxValue, ErrorMessage = DEFAULT_0_MSG)]
    public decimal Cn { get; set; }
  }

  public enum TransmissionType
  {
    Mecanique = 0,
    Automatique = 1
  }

  public enum MotorType
  {
    Essence = 0,
    Diesel = 1
  }

  public enum VehicleType
  {
    Tourisme = 0,
    PoidsLourd = 1
  }

}