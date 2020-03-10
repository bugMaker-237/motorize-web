namespace Motorize.Components
{
	using Blazorise;
	using Blazorise.Charts;
	using Microsoft.AspNetCore.Components;
	using Motorize.Models;
	using System;
	using System.Collections.Generic;
  using System.ComponentModel;
  using System.Diagnostics;
	using System.Linq;
  using System.Threading.Tasks;

  public partial class ChartValues
	{
		private Validations Validations;
		private Validations nvValidation;
		private LineChart<decimal> lineChart;
		private Modal modalRef;
		private Alert myAlert;
		private Action<decimal> onModalClosed;
		private bool ExpandAll = false;
		private decimal ValueNV = 0;
		private ChartValuesViewModel VM = new ChartValuesViewModel();
		private Dictionary<int, string> TransmissionTypes;
		private Dictionary<int, string> MotorTypes;
		private Dictionary<int, string> VehicleTypes;
		[Inject]
		ChartValuesState State { get; set; }

		protected override void OnInitialized()
		{
			this.TransmissionTypes = this.EnumToDictionary<TransmissionType>();
			this.MotorTypes = this.EnumToDictionary<MotorType>();
			this.VehicleTypes = this.EnumToDictionary<VehicleType>();
			this.VM = this.GetVM();
		}
		protected override Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				this.InitializeChart();
			}
			return base.OnAfterRenderAsync(firstRender);
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
				α = 20,
				Ψmax = 0.35M
			},
			EmpiricCoefficient = new EmpiricCoefficient
			{
				An = 0.53M,
				Bn = 1.56M,
				Cn = 1.09M
			},
			VehicleXtics = new VehicleXticsViewModel
			{
				TypeMoteur = (int)MotorType.Diesel,
				TypeTransmission = (int)TransmissionType.Automatique,
				TypeVehicule = (int)VehicleType.Tourisme,
				Vehicle = "Toyota Corola"
			}
		};


		private async void ShowModal(List<Tuple<decimal, decimal>> toDraw, Action<decimal> onModalClosed)
		{
			this.onModalClosed = onModalClosed;
			this.lineChart.Clear();
			var max = (toDraw.Select(i => i.Item1).Max() + 500).ToString();
			var all = toDraw.Select(i => i.Item1.ToString()).ToList();
			all.Add(max);
			this.lineChart.AddLabel(all.ToArray());
			this.lineChart.AddDataSet(new LineChartDataset<decimal>
			{
				Label = "Ne graph",
				Data = toDraw.Select(i => i.Item2).ToList(),
				BackgroundColor = new List<string>() { "#5B9BD5" },
				BorderColor = new List<string>() { "#5B9BD5" },
				Fill = false,
				PointRadius = 3,
				BorderDash = new List<int> { }
			});
			await this.lineChart.Update();
			this.modalRef.Show();
		}

		private void InitializeChart()
		{
			if (this.lineChart != null)
			{
				this.lineChart.Clear();
				this.lineChart.AddLabel("0");
				this.lineChart.AddDataSet(new LineChartDataset<decimal>
				{
					Label = "Ne Graph",
					Data = new List<decimal>()
				});
			}
		}
		private void HideModal()
		{
			if (this.nvValidation.ValidateAll())
			{
				this.modalRef.Hide();
				this.onModalClosed?.Invoke(this.ValueNV);
			}
		}
		private void OnModalClosing(CancelEventArgs e)
		{
			if (!this.nvValidation.ValidateAll())
			{
				e.Cancel = true;
			}
			else
			{
				this.onModalClosed?.Invoke(this.ValueNV);
			}
		}
		private Dictionary<int, string> EnumToDictionary<T>() where T : Enum
		{
			var type = typeof(T);
			return Enum.GetNames(type).ToDictionary(t => (int)Enum.Parse(type, t), t => t);
		}

		void Submit()
		{
			var res = this.Validations.ValidateAll();
			if (!res)
			{
				this.ExpandAll = true;
			}
			else
			{
				if (this.Test(this.VM))
				{
					this.BeginCalculations(this.VM, this.State.SetD);
				}
				else
				{
					this.myAlert.Show();
				}
			}
		}
		void Reset()
		{
			this.InitializeChart();
			this.State.SetD(null);
			this.VM = new ChartValuesViewModel();
		}
		private bool Test(ChartValuesViewModel VM)
		{
			bool isVT = VM.Data.Rk >= 0.25M &&
									VM.Data.Rk <= 0.39M &&
									VM.VehicleXtics.TypeVehicule == (int)VehicleType.Tourisme &&
									VM.Data.B <= 2 &&
									VM.Data.H <= 2.45M &&
									VM.Data.Kv >= 0.2M &&
									VM.Data.Kv <= 0.35M &&
									VM.Data.mo <= 1750;
			bool isVU = VM.Data.Rk >= 0.39M &&
									VM.Data.Rk <= 0.57M &&
									VM.VehicleXtics.TypeVehicule == (int)VehicleType.PoidsLourd &&
									VM.Data.B <= 2.35M &&
									VM.Data.H <= 4 &&
									VM.Data.Kv >= 0.6M &&
									VM.Data.Kv <= 0.7M &&
									VM.Data.mo <= 17000;
			Debug.WriteLine("isVT : " + isVT);
			Debug.WriteLine("isVU : " + isVU);

			return isVT || isVU;
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
		private void BeginCalculations(ChartValuesViewModel VM, Action<List<Tuple<decimal, decimal>>[]> onCalculationsEnd)
		{
			Calculator calculator = new Calculator();
			ChartValuesViewModel cvvm = VM;
			var ibvc = calculator.GetRapportVitesseComplementaire();
			decimal[][] velocities = this.GetVelocities();
			decimal[] valuesOfN = new decimal[] { 500, 1300, 2100, 2900, 2700, 4400, 5200, 5400 };

			//1: Calculate Mk (Couple au regime nominal)
			var Mkn = calculator.CalculateCoupleRegimeNominal(cvvm.Data.Nmax, cvvm.Data.nN);

			//2: Calculate Mr (Reserve du couple)
			var Mr = calculator.CalculateReserveCouple(Mkn, cvvm.Data.Mkmax);

			//3: Calculate k-omega (Coefficient d'adaptabilité)
			var ko = calculator.CalculateCoefficientAdaptabiliteRegime(cvvm.Data.nN, cvvm.Data.nMkmax);

			//4: Calculate km (Coefficient d'adaptabilité par rapport au couple)
			var km = calculator.CalculateCoefficientAdaptabiliteCouple(Mkn, cvvm.Data.Mkmax);

			//5: Calculate Hg (Centre de masse)
			var Hg = calculator.CalculateCentreMasse(cvvm.Data.H);

			//6: Calculate Omega-v (Coefficient de resistance total)
			var ov = calculator.CalculateCoefficientResistanceTotalDeplacement(calculator.GetCoefficientResistanceRoulement(), cvvm.Data.α);

			//7: Calculate F (Surface Frontal)
			var F = calculator.CalculateSurfaceFrontale(cvvm.Data.B, cvvm.Data.H);

			//8: Generate dataset for Ne and draw graph for user to determine nv

			//normally An, Bn and Cn are calculated, but for clients convenience they will be static

			var Ne = calculator.GetDatasetCourbePuissanceMoteur(cvvm.Data.Nmax, cvvm.Data.nN, cvvm.EmpiricCoefficient.An, cvvm.EmpiricCoefficient.Bn, cvvm.EmpiricCoefficient.Cn, valuesOfN);


			this.ShowModal(Ne, (nv) =>
			{

				//10: Calculate ibv1 (rapport 1er vitesse)
				var ip = calculator.CalculateRapportPont(cvvm.Data.Rk, nv, cvvm.Data.Vmax, ibvc);
				var first = calculator.CalculateRapportPremiereVitesse(cvvm.Data.mo * 9.81M, cvvm.Data.Ψmax, cvvm.Data.Rk, cvvm.Data.Mkmax, ibvc, ip, cvvm.Data.nt, calculator.GetCoefficientCorrection());

				//11: Calculate ibvi (rapport autre vitesse)
				decimal second = calculator.CalculateRapportAutreVitesse(6, 2, first);
				decimal third = calculator.CalculateRapportAutreVitesse(6, 3, first);
				decimal fourth = calculator.CalculateRapportAutreVitesse(6, 4, first);
				decimal fifth = calculator.CalculateRapportAutreVitesse(6, 5, first);
				decimal sixth = calculator.CalculateRapportAutreVitesse(6, 6, first);

				//12: Calculate iti
				decimal[] iti = new decimal[]{
					calculator.CalculateRapportGlobauxTransmission(first, ip),
					calculator.CalculateRapportGlobauxTransmission(second, ip),
					calculator.CalculateRapportGlobauxTransmission(third, ip),
					calculator.CalculateRapportGlobauxTransmission(fourth, ip),
					calculator.CalculateRapportGlobauxTransmission(fifth, ip),
					calculator.CalculateRapportGlobauxTransmission(sixth, ip),
				};

				//13: Generate matrix for Nei
				List<Tuple<decimal, decimal>>[] Nei = new List<Tuple<decimal, decimal>>[velocities.Length];

				for (int i = 0; i < velocities.Length; i++)
				{
					Nei[i] = calculator.GetDatasetRegimeMoteurChaqueRapport(iti[i], cvvm.Data.Rk, velocities[i]);
				}

				//9: Generate dataset for Mk 
				List<Tuple<decimal, decimal>>[] Mk = new List<Tuple<decimal, decimal>>[Nei.Length];

				for (int i = 0; i < velocities.Length; i++)
				{
					Mk[i] = calculator.GetDatasetCourbeCoupleMoteur(cvvm.Data.Mkmax, cvvm.Data.nN, cvvm.EmpiricCoefficient.An, cvvm.EmpiricCoefficient.Bn, cvvm.EmpiricCoefficient.Cn, Nei[i]);
				}
				//14: Generate matrix for PT

				// TODO: Come back here to verify MK; From document Mk needs to change according to velocity
				// and rapport de vitesse but it is not changing thus values cannot be used effectively on formula
				//Mki
				List<Tuple<decimal, decimal>>[] Pt = new List<Tuple<decimal, decimal>>[Mk.Length];

				for (int i = 0; i < Mk.Length; i++)
				{
					Pt[i] = calculator.GetDatasetForceTractionMoteur(iti[i], cvvm.Data.Rk, cvvm.Data.nt, Mk[i]);
				}

				//15: Generate matrix for Pw

				List<Tuple<decimal, decimal>>[] Pw = new List<Tuple<decimal, decimal>>[velocities.Length];

				for (int i = 0; i < velocities.Length; i++)
				{
					Pw[i] = calculator.GetDatasetForceAerodynamiqueResistance(cvvm.Data.H, cvvm.Data.B, cvvm.Data.Cf, cvvm.Data.Kv, velocities[i]);
				}

				//15: Generate matrix for D

				List<Tuple<decimal, decimal>>[] D = new List<Tuple<decimal, decimal>>[velocities.Length];

				for (int i = 0; i < velocities.Length; i++)
				{
					D[i] = calculator.GetDatasetFacteurDynamique(Pt[i], Pw[i], cvvm.Data.mo);
				}

				onCalculationsEnd(D);
			});

		}
	}
}