// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using seackers.Controllers;
using seackers.Models;
using Azure.Storage.Blobs;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Globalization;

namespace seackers.Areas.Identity.Pages.Account.Manage
{
    [ValidateAntiForgeryToken]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IConfiguration _config;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IStringLocalizer<HomeController> localizer, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
            _config = config;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            // INI MOD Añadir parámetros del Centro Asociado

            public string Name { get; set; }

            public string ImgLogoUrl { get; set; }

            public string ImgHeadUrl { get; set; }

            public String Latitud { get; set; }

            public String Longitud { get; set; }

            public string Phone { get; set; }

            public string Url { get; set; }

            public IFormFile FLogo { get; set; }

            public IFormFile FHead { get; set; }


            // FIN MOD

        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };

            // Si el usuario identificado es un centro tendrá opción a acceder o establecer sus datos de centro
            if(await _userManager.IsInRoleAsync(user, "Centro"))
            {
                Input.Phone = phoneNumber;
                SeackersContext context = new SeackersContext();                
                DataUser dU = context.DataUsers.Where(x => x.UserId == user.Id).SingleOrDefault();
                if (dU != null) 
                {
                
                    Input.Name = dU.Name;
                    Input.Url = dU.Url;
                    Input.Latitud = dU.Latitud.HasValue ? dU.Latitud.Value.ToString(CultureInfo.InvariantCulture) : "";
                    Input.Longitud = dU.Longitud.HasValue ? dU.Longitud.Value.ToString(CultureInfo.InvariantCulture) : "";
                    Input.ImgHeadUrl = dU.ImgHeadUrl;
                    Input.ImgLogoUrl = dU.ImgLogoUrl;
                }
            }

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string captchaResponse = Request.Form["g-recaptcha-response"];

            Utils.ReCaptchaValidationResult resultCaptcha = Utils.IsValid(captchaResponse, _config);

            if (resultCaptcha.Success)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                if (!ModelState.IsValid)
                {
                    await LoadAsync(user);
                    return Page();
                }

                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set phone number.";
                        return RedirectToPage();
                    }
                }
                                
                Boolean isLogo = false;
                if (Input.FLogo != null)
                {
                    if (Input.FLogo.ContentType.StartsWith("image/") && Input.FLogo.Length < 1048576 && Input.FLogo.Length > 0)
                    {
                        // Si el archivo no es una imagen, haz algo, como devolver un mensaje de error
                        //return BadRequest("El archivo seleccionado no es una imagen válida.");


                        //using var stream = new StreamReader(Input.FLogo.OpenReadStream());
                        //var bytes = Encoding.UTF8.GetBytes(stream.ReadToEnd());

                        // Obtener la referencia a la cuenta de almacenamiento de Azure
                        string connectionString = _config.GetValue<string>("blb"); ;
                        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                        // Obtener la referencia al contenedor de blobs públicos existente
                        string nombreContenedor = "publicimages";
                        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(nombreContenedor);

                        // Generar un nombre único para el archivo de imagen
                        string nombreArchivo = "L" + user.Id + Path.GetExtension(Input.FLogo.FileName);

                        // Obtener una referencia al blob de destino en el contenedor
                        BlobClient blobClient = containerClient.GetBlobClient(nombreArchivo);

                        // Guardar los bytes de la imagen en el blob
                        using (var stream = Input.FLogo.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, overwrite: true);
                        }

                        // Generar la URL pública del blob
                        //string urlBlob = $"{blobClient.Uri.Scheme}://{blobClient.Uri.Host}/{blobClient.Uri.Segments[1]}{nombreContenedor}/{nombreArchivo}";                    
                        string urlBlob = $"{blobClient.Uri.Scheme}://{blobClient.Uri.Host}/{nombreContenedor}/{nombreArchivo}";

                        Input.ImgLogoUrl = urlBlob;
                        isLogo = true;
                    }
                }

                Boolean isHead = false;
                if (Input.FHead != null)
                {
                    if (Input.FHead.ContentType.StartsWith("image/") && Input.FHead.Length < 5242880 && Input.FHead.Length > 0)
                    {
                        // Si el archivo no es una imagen, haz algo, como devolver un mensaje de error
                        //return BadRequest("El archivo seleccionado no es una imagen válida o es superior a 5 Mb.");


                        // Obtener la referencia a la cuenta de almacenamiento de Azure
                        string connectionString = _config.GetValue<string>("blb"); ;
                        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                        // Obtener la referencia al contenedor de blobs públicos existente
                        string nombreContenedor = "publicimages";
                        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(nombreContenedor);

                        // Generar un nombre único para el archivo de imagen
                        string nombreArchivo = "H" + user.Id + Path.GetExtension(Input.FHead.FileName);

                        // Obtener una referencia al blob de destino en el contenedor
                        BlobClient blobClient = containerClient.GetBlobClient(nombreArchivo);

                        // Guardar los bytes de la imagen en el blob
                        using (var stream = Input.FHead.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, overwrite: true);
                        }

                        // Generar la URL pública del blob
                        //string urlBlob = $"{blobClient.Uri.Scheme}://{blobClient.Uri.Host}/{blobClient.Uri.Segments[1]}{nombreContenedor}/{nombreArchivo}";                    
                        string urlBlob = $"{blobClient.Uri.Scheme}://{blobClient.Uri.Host}/{nombreContenedor}/{nombreArchivo}";

                        Input.ImgHeadUrl = urlBlob;
                        isHead = true;
                    }
                }


                SeackersContext context = new SeackersContext();
                DataUser u = context.DataUsers.Where(x => x.UserId == user.Id).SingleOrDefault();

                if(u == null)
                {
                    u = new DataUser() { UserId = user.Id,
                        Name = Input.Name,
                        Phone = Input.PhoneNumber,
                        Url = Input.Url,
                        Latitud = String.IsNullOrEmpty(Input.Latitud) ? null : Convert.ToDecimal(Input.Latitud, CultureInfo.InvariantCulture),
                        Longitud = String.IsNullOrEmpty(Input.Longitud) ? null : Convert.ToDecimal(Input.Longitud, CultureInfo.InvariantCulture),                     

                    };

                    if (isLogo)
                    {
                        u.ImgLogoUrl = Input.ImgLogoUrl;
                    }

                    if (isHead)
                    {
                        u.ImgHeadUrl = Input.ImgHeadUrl;
                    }

                    context.DataUsers.Add(u);
                }
                else
                {
                    u.Name = Input.Name;
                    u.Phone = Input.PhoneNumber;
                    u.Url = Input.Url;
                    u.Latitud = String.IsNullOrEmpty(Input.Latitud) ? null : Convert.ToDecimal(Input.Latitud, CultureInfo.InvariantCulture);
                    u.Longitud = String.IsNullOrEmpty(Input.Longitud) ? null : Convert.ToDecimal(Input.Longitud, CultureInfo.InvariantCulture);

                    if (isLogo)
                    {
                        u.ImgLogoUrl = Input.ImgLogoUrl;
                    }
                    if (isHead)
                    {
                        u.ImgHeadUrl = Input.ImgHeadUrl;
                    }
                }

                int r = context.SaveChanges();



                await _signInManager.RefreshSignInAsync(user);               
                StatusMessage = _localizer["profileUpdated"]; //"Your profile has been updated";
                return RedirectToPage();
            }
            else
            {
                StatusMessage = "Wrong captcha.";
                return RedirectToPage();
            }
        }
    }
}
