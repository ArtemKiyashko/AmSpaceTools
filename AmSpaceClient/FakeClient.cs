using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using AmSpaceModels;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace AmSpaceClient
{
    public class FakeClient : IAmSpaceClient
    {
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

        public Task<IEnumerable<Level>> GetLevelsAsync()
        {
            IEnumerable<Level> result = new List<Level>
            {
                new Level
                {
                    Id = 1,
                    Name = "1"
                },
                new Level
                {
                    Id = 2,
                    Name = "2"
                },
                new Level
                {
                    Id = 3,
                    Name = "3"
                }
            };
            return Task.FromResult(result);
        }

        public Task<bool> LoginRequestAsync(string userName, SecureString password, IAmSpaceEnvironment environment) => Task.FromResult(true);

        public async Task<bool> LogoutRequestAsync()
        {
            return await Task.FromResult(true);
        }

        public Task<Profile> ProfileRequestAsync(int? profileId = null)
        {
            ContractDatum fakeContract = new ContractDatum{
                Position = new Position
                {
                    Name = "Fake Department"
                }
            };
            ContractDatum[] fakeContractData = { fakeContract };

            var result = new Profile
            {
                FirstName = "MyName",
                LastName = "MySurname",
                ContractData = fakeContractData,
                Avatar = "https://pp.userapi.com/c637631/v637631947/5048d/vRj7_OW0f9U.jpg"
            };
            return Task.FromResult(result);
        }

        public Task<bool> UpdateActionAsync(UpdateAction model, long competencyId)
        {
            return Task.FromResult(true);
        }

        public async Task<BitmapSource> GetAvatarAsync(string link)
        {
            var client = new HttpClient();
            var result = await client.GetAsync(link);
            var content = await result.Content.ReadAsByteArrayAsync();
            return (BitmapSource)new ImageSourceConverter().ConvertFrom(content);
        }

        public Task<IEnumerable<AmspaceDomain>> GetOrganizationStructureAsync()
        {
            IEnumerable<AmspaceDomain> result = new List<AmspaceDomain>
            {
                new AmspaceDomain
                {
                    Name = "Wock and Roll",
                    Id = 1,
                    Children = new List<AmspaceDomain>
                    {
                        new AmspaceDomain
                        {
                            Name = "District 1",
                            Id = 2,
                            Children = new List<AmspaceDomain>
                            {
                                new AmspaceDomain
                                {
                                    Name = "Region 1",
                                    Id = 3,
                                    Children = new List<AmspaceDomain>
                                    {
                                        new AmspaceDomain
                                        {
                                            Name = "Restaurant 1",
                                            Id = 4,
                                            Children = new List<AmspaceDomain>
                                            {
                                                new AmspaceDomain
                                                {
                                                    Name = "Restaurant 2",
                                                    Id = 5,
                                                    Children = null
                                                },
                                                new AmspaceDomain
                                                {
                                                    Name = "Restaurant 3",
                                                    Id = 6,
                                                    Children = null
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return Task.FromResult(result);
        }

        public Task<IEnumerable<AmspaceUser>> GetDomainUsersAsync(int domainId)
        {
            IEnumerable<AmspaceUser> result = new List<AmspaceUser>()
            {
                new AmspaceUser
                {
                    Id = 1,
                    Name = "Jan Kovalski"
                },
                new AmspaceUser()
                {
                    Id = 2,
                    Name = "Emma Watson"
                },
                new AmspaceUser
                {
                    Id = 3,
                    Name = "Harry Potter"
                }
            };
            return Task.FromResult(result);
        }

        public Task<bool> PutUserAsync(SapUser user)
        {
            return Task.FromResult(true);
        }

        public Task<bool> PutDomainAsync(SapDomain domain)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DisableUserAsync(SapUserDelete user)
        {
            return Task.FromResult(true);
        }

        public Task<BitmapSource> GetAvatarAsync(string link, IImageConverter converter = null)
        {
            throw new NotImplementedException();
        }
    }
}
