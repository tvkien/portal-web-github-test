using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;
namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSProfileLinkRepository : ITLDSProfileLinkRepository
    {
        private readonly Table<TLDSProfileLinkEntity> table;
        private readonly Table<TLDSProfileEntity> tableProfile;
        private readonly TLDSContextDataContext _tldsContext;
        public TLDSProfileLinkRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _tldsContext = TLDSContextDataContext.Get(connectionString);
            table = _tldsContext.GetTable<TLDSProfileLinkEntity>();
            tableProfile = _tldsContext.GetTable<TLDSProfileEntity>();
        }

        public void Delete(TLDSProfileLink item)
        {
            throw new System.NotImplementedException();
        }

        public void Save(TLDSProfileLink item)
        {
            var entity = table.FirstOrDefault(x => x.TLDSProfileLinkID == item.TLDSProfileLinkID);
            if (entity == null)
            {                
                entity = new TLDSProfileLinkEntity();
                entity.TLDSProfileLinkID = item.TLDSProfileLinkID;
                table.InsertOnSubmit(entity);
            }

            entity.ProfileID = item.ProfileId;
            entity.LinkUrl = item.LinkUrl;
            entity.ExpiredDate = item.ExpiredDate;
            entity.ModifiedDate = item.ModifiedDate;
            entity.IsActive = item.IsActive;
            table.Context.SubmitChanges();            
        }

        public IQueryable<TLDSProfileLink> Select()
        {
            var query = table.Select(x => new TLDSProfileLink
            {
                TLDSProfileLinkID = x.TLDSProfileLinkID,
                ProfileId = x.ProfileID,
                LinkUrl = x.LinkUrl,
                ExpiredDate = x.ExpiredDate,
                ModifiedDate = x.ModifiedDate.HasValue ? x.ModifiedDate : null,
                IsActive = x.IsActive,
                LoginFailed = x.LoginFailed
            });
            
            return query;
        }

        public List<TLDSProfileLink> GetTLDSProfileLink(string scheme, int profileId, int userId, int? enrolmentYear, int? tldsGroupID)
        {
            var tldsProfileLinks = _tldsContext.GetTLDSProfileLink(userId, profileId, enrolmentYear, tldsGroupID)
                                    .Select(o => new TLDSProfileLink
                                    {
                                        ProfileId = o.ProfileID,
                                        StudentFirstName = o.StudentFirstName,
                                        StudentLastName = o.StudentLastName,
                                        Guardian = o.GuadianName,
                                        SectionCompleted = o.SectionCompleted,
                                        LinkUrl = string.Format("{0}://{1}/TLDSDigitalSection23?id={2}", scheme, o.LinkUrl, o.TLDSProfileLinkId),
                                        ExpiredDate = o.ExpiredDate,
                                        Status = o.Status,
                                        IsShowDeactivate = o.Status != Constanst.TLDSProfileLink_Deactivated,
                                        IsShowActivate = o.Status == Constanst.TLDSProfileLink_Deactivated,
                                        IsShowRefresh = (o.Status == Constanst.TLDSProfileLink_Open || o.Status == Constanst.TLDSProfileLink_InProgress
                                                        || o.Status == Constanst.TLDSProfileLink_Expired || o.Status == Constanst.TLDSProfileLink_Completed),
                                        TLDSProfileLinkID = o.TLDSProfileLinkId.GetValueOrDefault(),
                                        ProfileStatus = o.ProfileStatus.GetValueOrDefault(),
                                        EnrolmentYear = o.EnrolmentYear.GetValueOrDefault(),
                                        TLDSGroupID = o.TLDSGroupID.GetValueOrDefault(),
                                    })
                                    .ToList();
            return tldsProfileLinks;
        }      

        public bool UpdateTLDSProfileLink(Guid tldsProfileLinkID, bool value)
        {
            var query = table.FirstOrDefault(x => x.TLDSProfileLinkID == tldsProfileLinkID);
            if (query != null)
            {
                query.IsActive = value;
                if (value)
                    query.LoginFailed = 0;
                query.ModifiedDate = DateTime.UtcNow;
                table.Context.SubmitChanges();                
                return true;
            }
            return false;
        }

        public bool RefreshTLDSProfileLink(Guid tldsProfileLinkID, int day)
        {
            var query = table.FirstOrDefault(x => x.TLDSProfileLinkID == tldsProfileLinkID);
            if (query != null)
            {
                var addDate = (DateTime.UtcNow - query.ExpiredDate).TotalDays;
                // addDate >= day nothing
                if ((addDate > 0 && addDate < day) || addDate >= day)
                {
                    query.ExpiredDate = DateTime.UtcNow.AddDays(day);
                }
                else if(addDate == 0)
                {
                    query.ExpiredDate = query.ExpiredDate.AddDays(day);
                }
                else if(addDate < 0 && addDate > -day)
                {
                    query.ExpiredDate = query.ExpiredDate.AddDays(day + addDate);
                }

                query.LoginFailed = 0;
                query.ModifiedDate = DateTime.UtcNow;
                table.Context.SubmitChanges();                
                return true;
            }
            return false;
        }

        public void ResetLoginFailCount(Guid tldsProfileLinkID)
        {
            var query = table.FirstOrDefault(x => x.TLDSProfileLinkID == tldsProfileLinkID);
            if (query != null)
            {
                query.LoginFailed = 0;
                query.ModifiedDate = DateTime.UtcNow;
                table.Context.SubmitChanges();                
            }
        }

        public bool CheckTLDSFormSectionSubmitted(int profileId, int sectionType)
        {
            var query = _tldsContext.CheckTLDSSectionSubmitted(profileId, sectionType).FirstOrDefault();
            return (query != null && query.Result > 0);
        }

        public TLDSInformationToSendMail GetTLDSInformationForSection23(Guid tldsProfileLinkId)
        {
            return _tldsContext.GetTLDSInformationForSection23(tldsProfileLinkId).Select(x => new TLDSInformationToSendMail
            {
                TLDSProfileLinkId = x.TLDSProfileLinkId,
                ProfileId = x.ProfileID,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                LinkUrl = x.LinkUrl
            }).FirstOrDefault();
        }

        public int UpdateLoginFail(Guid tldsProfileLinkID, int loginLimit)
        {
            var query = table.FirstOrDefault(x => x.TLDSProfileLinkID == tldsProfileLinkID);
            if(query != null)
            {
                query.LoginFailed += 1;
                if(query.LoginFailed == loginLimit)
                {
                    query.IsActive = false;
                }
                table.Context.SubmitChanges();                
                return query.LoginFailed;
            }
            return 0;

        }

        public bool LoginTLDSForm(Guid id, DateTime dateOfBirth)
        {
            int? numOfProfile = 0;
            var query = _tldsContext.TLDSDigitalLogin(id, dateOfBirth, ref numOfProfile).First();
            return query.Column1 > 0;
        }

        public TLDSProfile GetTLDSProfileByTLDSProfileLinkId(Guid id)
        {
            return _tldsContext.GetTLDSProfileByTLDSProfileLinkId(id).Select(o => new TLDSProfile
                                                                    {
                                                                       ProfileId = o.ProfileID,
                                                                       FirstName = o.FirstName,
                                                                       LastName = o.LastName,
                                                                       DateOfBirth = o.DateOfBirth,
                                                                       EnrolmentYear = o.EnrolmentYear,
                                                                       PrimarySchool = o.PrimarySchool,
                                                                       OutsideSchoolHoursCareService = o.OutsideSchoolHoursCareService,
                                                                       PhotoURL = o.PhotoURL,
                                                                       Status = o.Status
                                                                    })
                                                                    .FirstOrDefault();
        }

        public void DeleteTldsProfileLink(Guid tldsProfileLinkId)
        {
            _tldsContext.TLDSProfileLink_Delete(tldsProfileLinkId);
        }
    }
}
