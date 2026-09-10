using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Devices;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


/// <summary>Suma, Resta, Multiplicación y División de números enteros grandes</summary>
namespace BigIntegersCalculator
{
    /// <summary>Página principal Big Integer Calculator</summary>
    public sealed partial class MainPage : Page
    {
        Byte[] tempFile = null;
        //static String appID = "5f65e5b90bff050001859d49";
        //VungleAd sdkInstance = AdFactory.GetInstance(appID);
        Boolean existeN = false;
        
        public MainPage()
        {
            this.InitializeComponent();

            //sdkInstance.OnInitCompleted += SdkInstance_OnInitCompleted;
            //sdkInstance.OnAdPlayableChanged += SdkInstance_OnAdPlayableChanged;
        }

        //private async void SdkInstance_OnAdPlayableChanged(object sender, AdPlayableEventArgs e)
        //{
        //    await sdkInstance.PlayAdAsync(new AdConfig(), "INTERSTICIAL-8854769");
        //}

        //private void SdkInstance_OnInitCompleted(object sender, ConfigEventArgs e)
        //{
        //    sdkInstance.LoadAd("INTERSTICIAL-8854769");
        //}

        /// <summary>Ocurre al clicar el botón Sumar</summary>
        private async void btnResolve_Click(object sender, RoutedEventArgs e)
        {
            tempFile = null;
            disableButtons();
            pBProcess.IsIndeterminate = true;
            String formula = txtFormula.Text != String.Empty && txtFormula.Text != "-" ? txtFormula.Text : "0";            
            lblResults.Text = "Calculando / Calculating";
            BigInteger nDesde = BigInteger.Parse(String.IsNullOrEmpty(txtNDesde.Text) || txtNDesde.Text == "-" ? "1" : txtNDesde.Text);
            BigInteger nHasta = BigInteger.Parse(String.IsNullOrEmpty(txtNHasta.Text) || txtNHasta.Text == "-" ? "1" : txtNHasta.Text);
            try
            {
                String result = await Task.Factory.StartNew(() => CalculoAlgebraico.CalculoAritmetico(formula, existeN, nDesde, nHasta, false));
                lblResults.Text = result;
            }
            catch (Exception ex)
            {
                lblResults.Text = ex.Message;
            }
            
            pBProcess.IsIndeterminate = false;
            activeButtons();
        }

        private async void MostrarGrafica_Click(object sender, RoutedEventArgs e)
        {
            String formula = txtFormula.Text != String.Empty && txtFormula.Text != "-" ? txtFormula.Text : "0";            
            BigInteger nDesde = BigInteger.Parse(String.IsNullOrEmpty(txtNDesde.Text) || txtNDesde.Text == "-" ? "1" : txtNDesde.Text);
            BigInteger nHasta = BigInteger.Parse(String.IsNullOrEmpty(txtNHasta.Text) || txtNHasta.Text == "-" ? "1" : txtNHasta.Text);
            try
            { 
                await OpenPageAsWindowAsync(typeof(Grafica), formula, nDesde, nHasta);
            }
            catch (Exception ex)
            {
                lblResults.Text = ex.Message;
            }
        }

        /// <summary>Ocurre al clicar el botón Comparar</summary>
        private void btnCom_Click(object sender, RoutedEventArgs e)
        {
            tempFile = null;
            disableButtons();

            txtN1.Text = txtN1.Text == "-" ? String.Empty : txtN1.Text;
            txtN2.Text = txtN2.Text == "-" ? String.Empty : txtN2.Text;

            Boolean? result = Compare(txtN1.Text, txtN2.Text);

            if (result.HasValue)
            {
                if (result.Value)
                {
                    lblResults.Text = "N1 es mayor que N2 / N1 is greater than N2";
                }
                else
                {
                    lblResults.Text = "N1 es menor que N2 / N1 is less than N2";
                }
            }
            else
            {
                lblResults.Text = "N1 y N2 son iguales / N1 and N2 are the same";
            }

            activeButtons();
        }

        /// <summary>Ocurre al clicar el botón Sumar</summary>
        private async void btnSum_Click(object sender, RoutedEventArgs e)
        {
            tempFile = null;
            disableButtons();
            pBProcess.IsIndeterminate = true;
            String n1 = txtN1.Text != String.Empty && txtN1.Text != "-" ? txtN1.Text : "0";
            String n2 = txtN2.Text != String.Empty && txtN2.Text != "-" ? txtN2.Text : "0";
            lblResults.Text = "Calculando / Calculating";            
            String result = await Task.Factory.StartNew(() => Sum2(n1, n2));
            lblResults.Text = result;
            pBProcess.IsIndeterminate = false;
            activeButtons();
        }

        /// <summary>Ocurre al clicar el botón Resta</summary>
        private async void btnRes_Click(object sender, RoutedEventArgs e)
        {
            tempFile = null;
            disableButtons();
            pBProcess.IsIndeterminate = true;
            String n1 = txtN1.Text != String.Empty && txtN1.Text != "-" ? txtN1.Text : "0";
            String n2 = txtN2.Text != String.Empty && txtN2.Text != "-" ? txtN2.Text : "0";
            lblResults.Text = "Calculando / Calculating";
            String result = await Task.Factory.StartNew(() => Res2(n1, n2));
            lblResults.Text = result;
            pBProcess.IsIndeterminate = false;
            activeButtons();
        }

        /// <summary>Ocurre al clicar el botón Multiplicación</summary>
        private async void btnMult_Click(object sender, RoutedEventArgs e)
        {
            tempFile = null;
            disableButtons();
            pBProcess.IsIndeterminate = true;
            lblResults.Text = "Calculando / Calculating";
            String n1 = txtN1.Text != String.Empty && txtN1.Text != "-" ? txtN1.Text : "0";
            String n2 = txtN2.Text != String.Empty && txtN2.Text != "-" ? txtN2.Text : "0";
            String result = await Task.Factory.StartNew(() => Mult2(n1, n2));
            lblResults.Text = result;
            pBProcess.IsIndeterminate = false;
            activeButtons();
        }

        /// <summary>Ocurre al clicar el botón División</summary>
        private async void btnDiv_Click(object sender, RoutedEventArgs e)
        {
            tempFile = null;
            disableButtons();
            pBProcess.IsIndeterminate = true;
            lblResults.Text = "Calculando / Calculating";
            String n1 = txtN1.Text != String.Empty && txtN1.Text != "-" ? txtN1.Text : "0";
            String n2 = txtN2.Text != String.Empty && txtN2.Text != "-" ? txtN2.Text : "0";
            DivisionEntera result = await Task.Factory.StartNew(() => Div2(n1, n2));
            lblResults.Text = result.EsExacta ? result.Resultado : result.Resultado + " resto / remainder " + result.Resto;
            pBProcess.IsIndeterminate = false;
            activeButtons();
        }

        /// <summary>Ocurre al clicar el botón Potencia</summary>
        private async void btnPot_Click(object sender, RoutedEventArgs e)
        {
            tempFile = null;
            disableButtons();
            pBProcess.IsIndeterminate = true;
            lblResults.Text = "Calculando / Calculating";
            String n1 = txtN1.Text != String.Empty && txtN1.Text != "-" ? txtN1.Text : "0";
            String n2 = txtN2.Text != String.Empty && txtN2.Text != "-" ? txtN2.Text : "0";
            try
            {
                PotenciaResult result = await Task.Factory.StartNew(() => Pot2(n1, n2));

                if (result.ResultB == null)
                {
                    lblResults.Text = result.ResultS;
                }
                else
                {
                    lblResults.Text = "Exporta a archivo para ver el resultado / Export to file to see the result";
                    tempFile = result.ResultB;
                }
            }
            catch (Exception ex)
            {
                lblResults.Text = ex.Message;
            }

            pBProcess.IsIndeterminate = false;
            activeButtons();
        }

        /// <summary>Suma dos números enteros en String</summary>
        public String Sum2(String n1, String n2)
        {
            BigInteger biN1 = BigInteger.Parse(n1);
            BigInteger biN2 = BigInteger.Parse(n2);

            BigInteger resultado = BigInteger.Add(biN1, biN2);

            return resultado.ToString();
        }

        public String Res2(String n1, String n2)
        {
            BigInteger biN1 = BigInteger.Parse(n1);
            BigInteger biN2 = BigInteger.Parse(n2);

            BigInteger resultado = BigInteger.Subtract(biN1, biN2);

            return resultado.ToString();
        }

        public String Mult2(String n1, String n2)
        {
            BigInteger biN1 = BigInteger.Parse(n1);
            BigInteger biN2 = BigInteger.Parse(n2);

            BigInteger resultado = BigInteger.Multiply(biN1, biN2);

            return resultado.ToString();
        }

        /// <summary>Divide dos números enteros en String</summary>
        public DivisionEntera Div2(String dividendo, String divisor)
        {
            try
            {
                BigInteger biN1 = BigInteger.Parse(dividendo);
                BigInteger biN2 = BigInteger.Parse(divisor);

                BigInteger multResto;
                BigInteger resultado = BigInteger.DivRem(biN1, biN2, out multResto);

                return new DivisionEntera() { Resultado = resultado.ToString(), EsExacta = multResto.IsZero ? true : false, Resto = multResto.IsZero ? String.Empty : multResto.ToString() };
            }
            catch (Exception ex)
            {
                return new DivisionEntera() { Resultado = "Infinito / Infinity", EsExacta = true };
            }
        }

        public PotenciaResult Pot2(String n1, String n2)
        {
            BigInteger biN1 = BigInteger.Parse(n1);
            BigInteger biN2 = BigInteger.Parse(n2);

            BigInteger result = new BigInteger(1);
            BigInteger deuda = biN2;

            Int32 compare = BigInteger.Compare(deuda, new BigInteger(50000000));
            BigInteger pow50000000 = new BigInteger(0);
            Boolean isFirst = true;
            while (compare > 0)
            {
                deuda -= new BigInteger(50000000);
                compare = BigInteger.Compare(deuda, new BigInteger(50000000));

                if(isFirst)
                {
                    pow50000000 = BigInteger.Pow(biN1, 50000000);
                    isFirst = false;
                }

                result *= pow50000000;
            }

            if(!deuda.IsZero)
            {
                result *= BigInteger.Pow(biN1, Convert.ToInt32(deuda.ToString()));
            }

            if (BigInteger.Compare(result, maxNum) < 1)
            {
                return new PotenciaResult() { ResultS = result.ToString(), ResultB = null };
            }
            else
            {
                return new PotenciaResult() { ResultB = UTF8Encoding.UTF8.GetBytes(result.ToString()) };                
            }
        }

        private async void btnFact_Click(object sender, RoutedEventArgs e)
        {
            tempFile = null;
            disableButtons();
            pBProcess.IsIndeterminate = true;
            lblResults.Text = "Calculando / Calculating";
            String n1 = txtN1.Text != String.Empty ? txtN1.Text : "0";
            String result = await Task.Factory.StartNew(() => factorial(n1));

            lblResults.Text = result;
            
            pBProcess.IsIndeterminate = false;
            activeButtons();
        }

        /// <summary>Método que calcula el factorial de un número BigInteger</summary>        
        private String factorial(String numS)
        {
            if (numS.Trim() == "1") return "1";

            BigInteger numero = BigInteger.Parse(numS);

            //Variable de la cuenta del factorial
            BigInteger resultado = new BigInteger(1);
            // Vamos multiplicando al número sus antecesores
            while (numero > 1)
            {
                resultado *= numero;
                numero--;
            }
            // Devolvemos el factorial
            return resultado.ToString();
        }

        /// <summary>Método que calcula el cambio de base de un número deciaml a un número base N2</summary>
        private async void btnBaseFrom10_Click(object sender, RoutedEventArgs e)
        {
            tempFile = null;
            disableButtons();
            pBProcess.IsIndeterminate = true;
            lblResults.Text = "Calculando / Calculating";
            try
            {
                String n1 = txtN1.Text != String.Empty ? txtN1.Text : "0";
                String n2 = txtN2.Text != String.Empty ? txtN2.Text : "2";

                Boolean esNegativo = false;
                if (n1.Contains("-"))
                {
                    esNegativo = true;
                    n1 = n1.Replace("-", "");
                }

                Int32 nBase = Convert.ToInt32(n2);
                if (nBase > 36) { throw new Exception("N2 o la base a convertir admite máximo, hasta la base 36 / N2 or the base to be converted, supports maximum up to base 36"); }
                if (nBase < 2) { throw new Exception("N2 o la base a convertir, debe ser mayor que 1 / N2 or the base to be converted, must be greater than 1"); };
                
                String result = await Task.Factory.StartNew(() => Criptografia.FromBase10(n1, nBase));

                lblResults.Text = String.Concat((esNegativo ? "-": "" ), result);
            }
            catch(Exception ex)
            {
                await new MessageDialog(ex.Message, "ERROR").ShowAsync();
            }
            pBProcess.IsIndeterminate = false;
            activeButtons();
        }

        /// <summary>Método que calcula el cambio de base de un número deciaml a un número base N2</summary>
        private async void btnBaseTo10_Click(object sender, RoutedEventArgs e)
        {
            tempFile = null;
            disableButtons();
            pBProcess.IsIndeterminate = true;
            lblResults.Text = "Calculando / Calculating";
            try
            {
                String n1 = txtN1.Text != String.Empty ? txtN1.Text : "0";
                String n2 = txtN2.Text != String.Empty ? txtN2.Text : "2";

                Boolean esNegativo = false;
                if (n1.Contains("-"))
                {
                    esNegativo = true;
                    n1 = n1.Replace("-", "");
                }

                Int32 nBase = Convert.ToInt32(n2);
                if (nBase > 36) { throw new Exception("N2 o la base a convertir admite máximo, hasta la base 36 / N2 or the base to be converted, supports maximum up to base 36"); }
                if (nBase < 2) { throw new Exception("N2 o la base a convertir, debe ser mayor que 1 / N2 or the base to be converted, must be greater than 1"); };

                String result = await Task.Factory.StartNew(() => Criptografia.ToBase10(n1, nBase));

                lblResults.Text = String.Concat((esNegativo ? "-" : ""), result);
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "ERROR").ShowAsync();
            }
            pBProcess.IsIndeterminate = false;
            activeButtons();
        }

        /// <summary>Compara dos números para comprobar si es mayor, menor o igual</summary>
        /// <returns>True si el principal es mayor, False si es menor o Null si son iguales</returns>
        public Boolean? Compare(String nPrincipal, String nSecundario)
        {
            String nPrin = nPrincipal.TrimStart(new char[1] { '0' });
            String nSec = nSecundario.TrimStart(new char[1] { '0' });

            if (nPrin == nSec)
            {
                return null;
            }
            else
            {
                if (nPrin.Length > nSec.Length)
                {
                    return true;
                }
                else if (nPrin.Length < nSec.Length)
                {
                    return false;
                }
                else
                {
                    for (Int32 i = 0; i < nPrin.Length; i++)
                    {
                        Int32 xPrin = Convert.ToInt32(nPrin[i].ToString());
                        Int32 xSec = Convert.ToInt32(nSec[i].ToString());

                        if (xPrin > xSec)
                        {
                            return true;
                        }
                        else if (xPrin < xSec)
                        {
                            return false;
                        }
                    }

                    return null;
                }
            }
        }

        /// <summary>Inicializa la barra de progreso con los datos iniciales en el hilo principal</summary>
        async void UpdateProgressBarMaximun(Int32 max)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                this.pBProcess.Maximum = max;
            });
        }

        /// <summary>Actualiza el valor de la barra de progreso con los datos del proceso</summary>
        async void UpdateProgressBarValue(Int32 i)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                this.pBProcess.Value = i;
            });
        }

        /// <summary>Desactiva los botones de cálculos</summary>
        private void disableButtons()
        {
            btnResolve.IsEnabled = false;
            btnCom.IsEnabled = false;
            btnSum.IsEnabled = false;
            btnRes.IsEnabled = false;
            btnMult.IsEnabled = false;
            btnDiv.IsEnabled = false;
            btnPot.IsEnabled = false;
            btnBaseFrom10.IsEnabled = false;
            btnBaseTo10.IsEnabled = false;
        }

        /// <summary>Activa los botones de cálculos</summary>
        private void activeButtons()
        {
            btnResolve.IsEnabled = true;
            btnCom.IsEnabled = true;
            btnSum.IsEnabled = true;
            btnRes.IsEnabled = true;
            btnMult.IsEnabled = true;
            btnDiv.IsEnabled = true;
            btnPot.IsEnabled = true;
            btnFact.IsEnabled = true;
            btnBaseFrom10.IsEnabled = true;
            btnBaseTo10.IsEnabled = true;
        }

        /// <summary>Guarda un archivo de texto con el resultado de las operación</summary>
        private async void SaveText_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation =
                PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "BigIntegerCalculatorFile";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                if (tempFile == null)
                {
                    await FileIO.WriteTextAsync(file, lblResults.Text);
                }
                else
                {
                    await FileIO.WriteBytesAsync(file, tempFile);
                }
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await CachedFileManager.CompleteUpdatesAsync(file);
                //if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                //{
                //    this.textBlock.Text = "File " + file.Name + " was saved.";
                //}
                //else
                //{
                //    this.textBlock.Text = "File " + file.Name + " couldn't be saved.";
                //}
            }
            //else
            //{
            //    this.textBlock.Text = "Operation cancelled.";
            //}
        }

        /// <summary>Comprueba la insercción de sólo dígitos en los cuadros de texto</summary>
        private void txt_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            Boolean cancel = false;

            for(Int32 i = 0; i < args.NewText.Length; i++)
            {
                if(args.NewText[i] == '-' && i != 0)
                {
                    cancel = true;
                    break;
                }
                else if(!Char.IsDigit(args.NewText[i]) && args.NewText[i] != '-')
                {
                    cancel = true;
                    break;
                }
            }

            args.Cancel = cancel;
            //args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        /// <summary>Comprueba la insercción de sólo dígitos en los cuadros de texto</summary>
        private void txt_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            String text = String.Empty;

            for (Int32 i = 0; i < sender.Text.Length; i++)
            {
                if (i == 0 && sender.Text[i] == '-')
                {
                    text += sender.Text[i];
                }
                else if (Char.IsDigit(sender.Text[i]))
                {
                    text += sender.Text[i];
                }
            }
            sender.Text = text;
            //sender.Text = new String(sender.Text.Where(char.IsDigit).ToArray());
        }
        private void txtFormula_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (sender.Text.Contains("n"))
            {
                existeN = true;
                spN.Visibility = Visibility.Visible;
                btnGrafica.Visibility = Visibility.Visible;
            }
            else
            {
                existeN = false;
                spN.Visibility = Visibility.Collapsed;
                btnGrafica.Visibility = Visibility.Collapsed;
            }
        }

        public BigInteger maxNum = new BigInteger(File.ReadAllBytes("2e1000000.xax"));

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            
        }

        private void btnAd_Click(object sender, RoutedEventArgs e)
        {
            //SdkInstance_OnInitCompleted(null, null);
        }


        private async Task<bool> OpenPageAsWindowAsync(Type t, String formula, BigInteger nDesde, BigInteger nHasta)
        {
            var view = CoreApplication.CreateNewView();            
            int id = 0;
            String datos = formula + ";" + CalculoAlgebraico.CalculoAritmetico(formula, true, nDesde, nHasta, true);

            await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                frame.Navigate(t, null);
                frame.DataContext = datos;
                Window.Current.Content = frame;
                Window.Current.Activate();
                id = ApplicationView.GetForCurrentView().Id;
            });

            return await ApplicationViewSwitcher.TryShowAsStandaloneAsync(id);
        }

        private async void Triangulos_Click(object sender, RoutedEventArgs e)
        {
            var view = CoreApplication.CreateNewView();
            int id = 0;            
            await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                frame.Navigate(typeof(Triangulos), null);                
                Window.Current.Content = frame;
                Window.Current.Activate();
                id = ApplicationView.GetForCurrentView().Id;
            });

            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(id);
        }

        private async void TextoCrypto_Click(object sender, RoutedEventArgs e)
        {
            var view = CoreApplication.CreateNewView();
            int id = 0;
            await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                frame.Navigate(typeof(TextoCrypto), null);
                Window.Current.Content = frame;
                Window.Current.Activate();
                id = ApplicationView.GetForCurrentView().Id;
            });

            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(id);
        }
    }
}
