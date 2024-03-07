using AdsProject.BussinessEntities;
using AdsProject.BussinessLogic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace AdsProject.GraphicUserInterface.Controllers
{
    public class UserController : Controller
    {
        UserBL userBL = new UserBL();
        RoleBL roleBL = new RoleBL();

        // acción que muestra la lista de usuarios registrados
        public async Task<IActionResult> Index(User user = null)
        {
            if (user == null)
                user = new User();
            if (user.Top_Aux == 0)
                user.Top_Aux = 10; // setear el número de registros a mostrar
            else if (user.Top_Aux == -1)
                user.Top_Aux = 0;

            var users = await userBL.SearchIncludeRoleAsync(user);
            var roles = await roleBL.GetAllAsync();

            ViewBag.Roles = roles;
            ViewBag.Top = user.Top_Aux;

            return View(users);
        }

        // acción que muestra el detalle de un registro
        public async Task<IActionResult> Details(int id)
        {
            var user = await userBL.GetByIdAsync(new User { Id = id });
            user.Role = await roleBL.GetByIdAsync(new Role { Id = user.IdRole });
            return View(user);
        }

        // acción que muestra el formulario para un registro nuevo
        public async Task<IActionResult> Create()
        {
            var roles = await roleBL.GetAllAsync();
            ViewBag.Roles = roles;
            return View();
        }

        // acción que recibe los datos y los envía a la bd mediante el modelo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            try
            {
                int result = await userBL.CreateAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.Roles = await roleBL.GetAllAsync();
                return View(user);
            }
        }

        // acción que muestra el formulario con los datos cargados para modificar
        public async Task<IActionResult> Edit(int id)
        {
            var user = await userBL.GetByIdAsync(new User{ Id = id });
            user.Role = await roleBL.GetByIdAsync(new Role { Id = user.Id });
            ViewBag.Roles = await roleBL.GetAllAsync();
            return View(user);
        }

        // acción que recibe los datos modificados y los envía a la bd
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            try
            {
                int result = await userBL.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.Roles = await roleBL.GetAllAsync();
                return View(user);
            }
        }

        // acción que muestra los datos para confirmar la eliminación
        public async Task<IActionResult> Delete(int id)
        {
            var user = await userBL.GetByIdAsync(new User { Id = id });
            user.Role = await roleBL.GetByIdAsync(new Role { Id = user.Id });

            return View(user);
        }

        // acción que recibe la confirmación para eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, User user)
        {
            try
            {
                int result = await userBL.DeleteAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                var userDb = await userBL.GetByIdAsync(user);
                if (userDb == null)
                    userDb = new User();
                if(userDb.Id > 0)
                    userDb.Role = await roleBL.GetByIdAsync(new Role { Id = userDb.Id });
                return View(userDb);
            }
        }

        // acción que muestra el formulario de inicio de sesión
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.Url = returnUrl;
            ViewBag.Error = "";
            return View();
        }




    }
}
