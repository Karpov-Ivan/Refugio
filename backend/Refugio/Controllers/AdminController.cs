using VKAPI;
using System;
using Refugio.Common;
using Microsoft.AspNetCore.Mvc;
using Refugio.Interfaces.Repository;
using Refugio.Common.Clusterization;
using Microsoft.AspNetCore.Authorization;

namespace Refugio.Controllers
{
    /// <summary>
    /// Контроллер для административных действий.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route(template: "api/[controller]")]
    public class AdminController : Controller
    {
        private readonly IGroupRepository _groupDb;

        private readonly IUserRepository _userDb;

        private readonly IConfiguration _configuration;

        private readonly SetInterests _interests;

        private readonly CombiningInterests _cmb;

        /// <summary>
        /// Конструктор контроллера администрирования.
        /// </summary>
        public AdminController(IGroupRepository groupDb,
                              IUserRepository userDb,
                              SetInterests interests,
                              CombiningInterests cmb)
        {
            _groupDb = groupDb;
            _userDb = userDb;
            _interests = interests;
            _cmb = cmb;
        }

        /// <summary>
        /// Инициализация базы данных.
        /// </summary>
        /// <returns>Объект IActionResult, представляющий ответ на инициализацию базы данных.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status403Forbidden">Выбрасывается, когда доступ запрещен.</exception>
        /// <exception cref="Status500InternalServerError">Выбрасывается, когда происходит внутренняя ошибка сервера в процессе инициализации базы данных.</exception>
        [HttpPost("Initialize")]
        public async Task<IActionResult> InitializeDB()
        {
            IActionResult response;

            try
            {
                var config = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .Build();

                var vk = new VkApiHandler(config["VK:GroupId_TypicalMSTU"],
                                          (ulong)Convert.ToInt32(config["VK:ApplicationId"]),
                                          config["VK:Token"]);

                var usersFromVk = vk.GetInformationAboutGroupUsersById();

                var count = 1;

                foreach (var user in usersFromVk)
                {
                    await _userDb.AddUser(user);

                    var groups = vk.GetUserSubscriptionInformationById(user.VkId.ToString());

                    if (groups == null)
                        continue;

                    foreach (var group in groups)
                    {
                        await _groupDb.AddGroup(group, user);
                    }
                }

                response = Ok();
            }
            catch (ArgumentException)
            {
                response = StatusCode(StatusCodes.Status403Forbidden);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Привязка интересов к пользователям.
        /// </summary>
        /// <returns>Объект IActionResult, представляющий ответ на назначение интересов пользователям.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status403Forbidden">Выбрасывается, когда доступ запрещен.</exception>
        /// <exception cref="Status500InternalServerError">Выбрасывается, когда происходит внутренняя ошибка сервера в процессе назначения интересов пользователям.</exception>
        [HttpPost("AssignInterestsToUsers")]
        public async Task<IActionResult> AssignInterestsToUsers()
        {
            IActionResult response;

            try
            {
                var activity = await _groupDb.GetAllActivity();

                var u = await _userDb.GetUsers();

                _interests.Activity();

                var user = await _userDb.GetUsers();

                _interests.SetActivities(user, _userDb, _cmb);

                response = Ok();
            }
            catch (ArgumentException)
            {
                response = StatusCode(StatusCodes.Status403Forbidden);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }
    }
}