using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AmSpaceModels;
using AmSpaceTools.Infrastructure;
using Newtonsoft.Json;

namespace AmSpaceClient
{
    public class AmSpaceClient : IAmSpaceClient
    {
        private CookieContainer _cookieContainer;
        private bool _isAthorized;
        private LoginResult _loginResult;
        private string _clientId;
        private string _grantPermissionType;
        private Uri _baseAddress;
        private ApiEndpoints _apiEndpoits;

        public CookieContainer CookieContainer { get => _cookieContainer; private set => _cookieContainer = value; }
        public bool IsAthorized { get => _isAthorized; private set => _isAthorized = value; }
        public LoginResult LoginResult { get => _loginResult; private set => _loginResult = value; }
        public string ClientId { get => _clientId; private set => _clientId = value; }
        public string GrantPermissionType { get => _grantPermissionType; private set => _grantPermissionType = value; }
        public Uri BaseAddress
        {
            get
            {
                return _baseAddress;
            }
            private set
            {
                _baseAddress = value;
                AmSpaceHttpClient.BaseAddress = value;
            }
        }
        public ApiEndpoints Endpoints { get => _apiEndpoits; private set => _apiEndpoits = value; }
        public HttpClient AmSpaceHttpClient { get; private set; }
        

        public AmSpaceClient()
        {
            CookieContainer = new CookieContainer();
            Endpoints = new ApiEndpoints();

            var handler = new HttpClientHandler()
            {
                CookieContainer = CookieContainer
            }; 

            AmSpaceHttpClient = new HttpClient(handler);
            IsAthorized = false;
        }
        
        public async Task<bool> LoginRequestAsync(string userName, SecureString password, IAmSpaceEnvironment environment)
        {
            if (IsAthorized) return true;
            BaseAddress = new Uri(environment.BaseAddress);
            ClientId = environment.ClientId;
            GrantPermissionType = environment.GrantPermissionType;
            var values = new Dictionary<string, string>
                {
                    { "username", userName },
                    { "password", password.ToInsecureString() },
                    { "grant_type", GrantPermissionType },
                    { "client_id", ClientId }
                };
            var content = new FormUrlEncodedContent(values);
            var result = await AmSpaceHttpClient.PostAsync(Endpoints.TokenEndpoint, content);
            if (result.StatusCode != HttpStatusCode.OK) return false;
            var resultContent = await result.Content.ReadAsStringAsync();
            LoginResult = JsonConvert.DeserializeObject<LoginResult>(resultContent);
            AddAuthHeaders();
            AddAuthCookies();
            IsAthorized = true;
            return true;
        }

        public async Task<BitmapSource> GetAvatarAsync(string link)
        {
            if (!IsAthorized) throw new UnauthorizedAccessException();
            var result = await AmSpaceHttpClient.GetAsync(link);
            if (!result.IsSuccessStatusCode)
                result = await AmSpaceHttpClient.GetAsync("/static/avatar.png");
            var content = await result.Content.ReadAsByteArrayAsync();
            return (BitmapSource)new ImageSourceConverter().ConvertFrom(content);
        }

        public Task<IEnumerable<Competency>> GetCompetenciesAsync()
        {
            IEnumerable<Competency> result = new List<Competency>
            {
                new Competency
                {
                    Id = 1,
                    Name = "Decision Making",
                    LevelId = 1
                },
                new Competency
                {
                    Id = 2,
                    Name = "Decision Making",
                    LevelId = 2
                },
                new Competency
                {
                    Id = 3,
                    Name = "Communication",
                    LevelId = 3
                }
            };
            return Task.FromResult(result);
        }

        public Task<CompetencyAction> GetCompetencyActionsAsync(long competencyId)
        {
            IEnumerable<CompetencyAction> result = new List<CompetencyAction>
            {
                new CompetencyAction
                {
                    Id = 1,
                    Name = "Decision Making",
                    LevelId = 1,
                    Actions = new List<IdpAction>
                    {
                        new IdpAction
                        {
                            Id = 1,
                            Name = "make deciscion dude",
                            ActionType = new ActionType
                            {
                                Value = 10,
                                DisplayName = "10%"
                            },
                            Translations = new List<Translation>
                            {
                                new Translation
                                {
                                    Id = 1,
                                    Name = "make deciscion dude",
                                    Language = "en-us"
                                },
                                new Translation
                                {
                                    Id = 2,
                                    Name = "something in Polish",
                                    Language = "pl-pl"
                                }
                            }
                        },
                        new IdpAction
                        {
                            Id = 2,
                            Name = "make deciscion dude again",
                            ActionType = new ActionType
                            {
                                Value = 20,
                                DisplayName = "20%"
                            },
                            Translations = new List<Translation>
                            {
                                new Translation
                                {
                                    Id = 3,
                                    Name = "make deciscion dude",
                                    Language = "en-us"
                                },
                                new Translation
                                {
                                    Id = 4,
                                    Name = "something in Polish",
                                    Language = "pl-pl"
                                }
                            }
                        }
                    }
                },
                new CompetencyAction
                {
                    Id = 2,
                    Name = "Decision Making",
                    LevelId = 2,
                    Actions = new List<IdpAction>
                    {
                        new IdpAction
                        {
                            Id = 3,
                            Name = "Analysis the problem in terms of: a) how things are (expected standards vs current situation) b) how I want things to be – what do you expected?",
                            ActionType = new ActionType
                            {
                                Value = 70,
                                DisplayName = "70%"
                            },
                            Translations = new List<Translation>
                            {
                                new Translation
                                {
                                    Id = 5,
                                    Name = "Analysis the problem in terms of: a) how things are (expected standards vs current situation) b) how I want things to be – what do you expected?",
                                    Language = "en"
                                },
                                new Translation
                                {
                                    Id = 51,
                                    Name = "2Przeanalizuj problem uwzględniając następujące aspekty: a) jak wygląda sytuacja (rzeczywistość kontra oczekiwany standard) b) jak chciałbyś, żeby wyglądała – czego oczekujesz?",
                                    Language = "pl"
                                },
                                new Translation
                                {
                                    Id = 52,
                                    Name = "Анализирайте проблема, като вземете предвид следните аспекти: а) каква е ситуацията (реалност спрямо очаквания стандарт) б) как бихте искали да изглежда ситуацията - какво очаквате?",
                                    Language = "bg"
                                }
                            }
                        },
                        new IdpAction
                        {
                            Id = 4,
                            Name = "make deciscion dude again",
                            ActionType = new ActionType
                            {
                                Value = 20,
                                DisplayName = "20%"
                            },
                            Translations = new List<Translation>
                            {
                                new Translation
                                {
                                    Id = 7,
                                    Name = "make deciscion dude",
                                    Language = "en-us"
                                },
                                new Translation
                                {
                                    Id = 8,
                                    Name = "something in Polish",
                                    Language = "pl-pl"
                                }
                            }
                        }
                    }
                },
                new CompetencyAction
                {
                    Id = 3,
                    Name = "Communication",
                    LevelId = 3,
                    Actions = new List<IdpAction>
                    {
                        new IdpAction
                        {
                            Id = 5,
                            Name = "make deciscion dude",
                            ActionType = new ActionType
                            {
                                Value = 10,
                                DisplayName = "10%"
                            },
                            Translations = new List<Translation>
                            {
                                new Translation
                                {
                                    Id = 9,
                                    Name = "make deciscion dude",
                                    Language = "en-us"
                                },
                                new Translation
                                {
                                    Id = 10,
                                    Name = "something in Polish",
                                    Language = "pl-pl"
                                }
                            }
                        },
                        new IdpAction
                        {
                            Id = 6,
                            Name = "make deciscion dude again",
                            ActionType = new ActionType
                            {
                                Value = 20,
                                DisplayName = "20%"
                            },
                            Translations = new List<Translation>
                            {
                                new Translation
                                {
                                    Id = 11,
                                    Name = "make deciscion dude",
                                    Language = "en-us"
                                },
                                new Translation
                                {
                                    Id = 12,
                                    Name = "something in Polish",
                                    Language = "pl-pl"
                                }
                            }
                        }
                    }
                }
            };
            return Task.FromResult(result.FirstOrDefault(_ => _.Id == competencyId));
        }

        public async Task<IEnumerable<Level>> GetLevelsAsync()
        {
            if (!IsAthorized) throw new UnauthorizedAccessException();
            var result = await AmSpaceHttpClient.GetAsync(Endpoints.LevelsEndpoint);
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Level>>(content);
        }

        public async Task<bool> LogoutRequestAsync()
        {
            if (!IsAthorized) return false;
            var values = new Dictionary<string, string>()
                {
                    { "token", LoginResult.AccessToken },
                    { "client_id", ClientId }
                };
            var content = new FormUrlEncodedContent(values);
            var result = await AmSpaceHttpClient.PostAsync(Endpoints.LogoutEndpoint, content);
            if (result.StatusCode != HttpStatusCode.OK) return false;
            return true;
        }

        public async Task<Profile> ProfileRequestAsync()
        {
            var result = AmSpaceHttpClient.GetAsync(Endpoints.ProfileEndpoint, HttpCompletionOption.ResponseContentRead);
            var stringResult = await result.Result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Profile>(stringResult);
        }

        public Task UpdateActionAsync(UpdateAction model, long competencyId)
        {
            return Task.CompletedTask;
        }

        private void AddAuthHeaders()
        {
            AmSpaceHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {LoginResult.AccessToken}");
        }

        private void AddAuthCookies()
        {
            CookieContainer.Add(_baseAddress, new Cookie("accessToken", LoginResult.AccessToken));
        }
    }
}
