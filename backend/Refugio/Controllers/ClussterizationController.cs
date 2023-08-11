using VKAPI;
using System;
using AutoMapper;
using Refugio.Dto;
using Refugio.Common;
using Refugio.Models;
using Microsoft.AspNetCore.Mvc;
using Refugio.Interfaces.Repository;
using Refugio.Common.Clusterization;

namespace RefugioApp.Controllers
{
    /// <summary>
    /// Контроллер для кластеризации данных пользователей.
    /// </summary>
    [ApiController]
    [Route(template: "api/[controller]")]
    public class ClussterizationController : Controller
    {
        private readonly IGroupRepository _groupDb;

        private readonly IUserRepository _userDb;

        private readonly IConfiguration _configuration;

        protected readonly IMapper _mapper;

        private readonly Clusterization_ _clust;

        private readonly SetInterests _interests;

        private readonly SetMetrics _metrics;

        private readonly CombiningInterests _cmb;

        private readonly ModelSaver _userWithActivities;

        /// <summary>
        /// Конструктор контроллера для кластеризации данных пользователей.
        /// </summary>
        public ClussterizationController(IGroupRepository groupDb,
                                         IUserRepository userDb,
                                         IMapper mapper,
                                         Clusterization_ clust,
                                         SetInterests interests,
                                         CombiningInterests cmb,
                                         SetMetrics metrics,
                                         ModelSaver modelSaver)
        {
            _groupDb = groupDb;
            _userDb = userDb;
            _mapper = mapper;
            _clust = clust;
            _interests = interests;
            _cmb = cmb;
            _metrics = metrics;
            _userWithActivities = modelSaver;
        }

        /// <summary>
        /// Устанавливает пользователя по заданной ссылке.
        /// </summary>
        /// <param name="Link">Ссылка на пользователя.</param>
        /// <returns>Объект IActionResult, представляющий ответ на установку пользователя.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status500InternalServerError">Выбрасывается, когда происходит внутренняя ошибка сервера в процессе установки пользователя.</exception>
        [HttpGet("SetUser/{Link}")]
        public async Task<IActionResult> SetUser(string Link)
        {
            IActionResult response;

            try
            {
                if (Link.Contains('%'))
                    Link = Link.Substring(Link.LastIndexOf('F') + 1);
                else if (Link.Contains('/'))
                    Link = Link.Substring(Link.LastIndexOf('/') + 1);

                var config = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .Build();

                var vk = new VkApiHandler(config["VK:GroupId_TypicalMSTU"],
                                          (ulong)Convert.ToInt32(config["VK:ApplicationId"]),
                                          config["VK:Token"]);

                var user = vk.GetUserInformationById(Link);

                var groupsUser = vk.GetUserSubscriptionInformationById(user.VkId.ToString());

                var interests = new SetInterests();

                interests.Activity();

                _userWithActivities.user = interests.SetActivitiesForOneUser(user, groupsUser, _cmb);

                response = Ok();
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Кластеризует интересы пользователей.
        /// </summary>
        /// <returns>Объект IActionResult, представляющий ответ на запрос кластеризации интересов.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status500InternalServerError">Выбрасывается, когда происходит внутренняя ошибка сервера в процессе кластеризации интересов.</exception>
        [HttpGet("InterestsClusters")]
        public async Task<IActionResult> InterestsClussterisation()
        {
            IActionResult response;

            try
            {
                var users = await _userDb.GetUsers();

                _interests.SetActivities(users, _userDb, _cmb);

                users.Add(_userWithActivities.user);

                var clusters = _clust.ClusteringUsers(users, new List<User>()
                {
                    _userWithActivities.user
                }, new List<int> { -1 });

                var points = _clust.GetListOfPoints(users, new List<int> { -1 });

                response = Ok(_mapper.Map<List<UserDto>>(clusters).Take(50));
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Кластеризует пользователей по университетам.
        /// </summary>
        /// <returns>Объект IActionResult, представляющий ответ на запрос кластеризации пользователей по университетам.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status500InternalServerError">Выбрасывается, когда происходит внутренняя ошибка сервера в процессе кластеризации пользователей по университетам.</exception>
        [HttpGet("UniversitiesClusters")]
        public async Task<IActionResult> UniversitiesClussterisation()
        {
            IActionResult response;

            try
            {
                if (_userWithActivities.user.University == null)
                    return StatusCode(StatusCodes.Status204NoContent);

                var users = await _userDb.GetUsers();

                _interests.SetActivities(users, _userDb, _cmb);

                users.Add(_userWithActivities.user);

                var secondCoord = _metrics.SetUniversitiesToNumbers(users);

                var clusters = _clust.ClusteringUsers(users, new List<User>()
                {
                    _userWithActivities.user
                }, secondCoord);

                response = Ok(_mapper.Map<List<UserDto>>(clusters).Take(50));
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Кластеризует пользователей по факультетам.
        /// </summary>
        /// <returns>Объект IActionResult, представляющий ответ на запрос кластеризации пользователей по факультетам.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status500InternalServerError">Выбрасывается, когда происходит внутренняя ошибка сервера в процессе кластеризации пользователей по факультетам.</exception>
        [HttpGet("FacultiesClussters")]
        public async Task<IActionResult> FacultiesClussterisation()
        {
            IActionResult response;

            try
            {
                if (_userWithActivities.user.FacultyName == null)
                    return StatusCode(StatusCodes.Status204NoContent);

                var users = await _userDb.GetUsers();

                _interests.SetActivities(users, _userDb, _cmb);

                users.Add(_userWithActivities.user);

                var secondCoord = _metrics.SetFacultiesToNumbers(users);

                var clusters = _clust.ClusteringUsers(users, new List<User>()
                {
                    _userWithActivities.user
                }, secondCoord);

                response = Ok(_mapper.Map<List<UserDto>>(clusters));
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Получает точки для кластеризации пользователей по интересам.
        /// </summary>
        /// <returns>Объект IActionResult, представляющий ответ с данными точек для кластеризации пользователей по интересам.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status500InternalServerError">Выбрасывается, когда происходит внутренняя ошибка сервера в процессе получения данных точек для кластеризации пользователей по интересам.</exception>
        [HttpGet("GetPointsInterests")]
        public async Task<IActionResult> GetPointsInterests()
        {
            IActionResult response;

            try
            {
                var users = await _userDb.GetUsers();

                _interests.SetActivities(users, _userDb, _cmb);

                users.Add(_userWithActivities.user);

                var clusters = _clust.GetClusters(users, new List<int> { -1 });

                var pointDto = new List<PointDto>();

                int count = 0;

                foreach (var cluster in clusters.Clusters)
                {
                    foreach (var point in cluster.Objects)
                    {
                        if (point._user.VkId == _userWithActivities.user.VkId)
                        {
                            var us = new PointDto()
                            {
                                pointX = point.Point.X,
                                pointY = point.Point.Y,
                                color = $"rgb({240}, {240 - count * 20}, {240})"
                            };

                            us.color = "rgb(240,7,7)";

                            pointDto.Add(us);
                        }
                        else
                        {
                            pointDto.Add(new PointDto()
                            {
                                pointX = point.Point.X,
                                pointY = point.Point.Y,
                                color = $"rgb({240}, {240 - count * 20}, {240})"
                            });
                        }
                    }
                    count++;
                }

                response = Ok(pointDto);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Получает точки для кластеризации пользователей по университетам.
        /// </summary>
        /// <returns>Объект IActionResult, представляющий ответ с данными точек для кластеризации пользователей по университетам.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status204NoContent">Выбрасывается, когда нет данных для кластеризации.</exception>
        /// <exception cref="Status500InternalServerError">Выбрасывается, когда происходит внутренняя ошибка сервера в процессе получения данных точек для кластеризации пользователей по университетам.</exception>
        [HttpGet("GetPointsUniversities")]
        public async Task<IActionResult> GetPointsUniversities()
        {
            IActionResult response;

            try
            {
                if (_userWithActivities.user.University == null)
                    return StatusCode(StatusCodes.Status204NoContent);

                var users = await _userDb.GetUsers();

                _interests.SetActivities(users, _userDb, _cmb);

                users.Add(_userWithActivities.user);

                var secondCoord = _metrics.SetUniversitiesToNumbers(users);

                var clusters = _clust.GetClusters(users, secondCoord);

                var pointDto = new List<PointDto>();

                int count = 0;

                foreach (var cluster in clusters.Clusters)
                {
                    foreach (var point in cluster.Objects)
                    {
                        if (point._user.VkId == _userWithActivities.user.VkId)
                        {
                            var us = new PointDto()
                            {
                                pointX = point.Point.X,
                                pointY = point.Point.Y,
                                color = $"rgb({240}, {240 - count * 20}, {240})"
                            };

                            us.color = "rgb(240,7,7)";

                            pointDto.Add(us);
                        }
                        else
                        {
                            pointDto.Add(new PointDto()
                            {
                                pointX = point.Point.X,
                                pointY = point.Point.Y,
                                color = $"rgb({240}, {240 - count * 20}, {240})"
                            });
                        }
                    }
                    count++;
                }

                response = Ok(pointDto);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Получает точки для кластеризации пользователей по факультетам.
        /// </summary>
        /// <returns>Объект IActionResult, представляющий ответ с данными точек для кластеризации пользователей по факультетам.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status204NoContent">Выбрасывается, когда нет данных для кластеризации.</exception>
        /// <exception cref="Status500InternalServerError">Выбрасывается, когда происходит внутренняя ошибка сервера в процессе получения данных точек для кластеризации пользователей по факультетам.</exception>
        [HttpGet("GetPointsFaculties")]
        public async Task<IActionResult> GetPointsFaculties()
        {
            IActionResult response;

            try
            {
                if (_userWithActivities.user.FacultyName == null)
                    return StatusCode(StatusCodes.Status204NoContent);

                var users = await _userDb.GetUsers();

                _interests.SetActivities(users, _userDb, _cmb);

                users.Add(_userWithActivities.user);

                var secondCoord = _metrics.SetFacultiesToNumbers(users);

                var clusters = _clust.GetClusters(users, secondCoord);

                var pointDto = new List<PointDto>();

                int count = 0;

                foreach (var cluster in clusters.Clusters)
                {
                    foreach (var point in cluster.Objects)
                    {
                        if (point._user.VkId == _userWithActivities.user.VkId)
                        {
                            var us = new PointDto()
                            {
                                pointX = point.Point.X,
                                pointY = point.Point.Y,
                                color = $"rgb({240}, {240 - count * 20}, {240})"
                            };

                            us.color = "rgb(240,7,7)";

                            pointDto.Add(us);
                        }
                        else
                        {
                            pointDto.Add(new PointDto()
                            {
                                pointX = point.Point.X,
                                pointY = point.Point.Y,
                                color = $"rgb({240}, {240 - count * 20}, {240})"
                            });
                        }
                    }
                    count++;
                }

                response = Ok(pointDto);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }
    }
}