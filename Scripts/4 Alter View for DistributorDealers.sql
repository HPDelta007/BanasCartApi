use [3DKDH]

GO

ALTER view [dbo].[DistributorDealers]
as
select d.Code as DistributorDealerId, 'Dealer' as DataType, d.Name as Name, m.StreetAddress as Address, 0 as Lat, 0 as Long, s.Name as States, ds.Name as District, c.Name as CityVillage
, replace(replace(d.FollowUpPerson, '[', '('), ']', ')') as ContactPersonName, d.MobileNo as WhatsAppMobileNo, m.TelephoneNos as OtherMobileNo, d.EmailId as EmailId, m.GSTNo as GSTNo, m.PANNo as PANNo, m.BankName AS BankName, m.AccNo AS BankACNo, m.IFSCCode AS BankIFSCCode, NULL AS ProfilePhoto, NULL AS CoverPhoto, NULL AS ParentDistributorDealerId, m.InsertedOn, m.LastUpdatedOn, m.InsertedByUserId, m.LastUpdatedByUserId 
from Debtors d
inner join KrishnaiERP..LgrAddresses  m on m.LgrId = d.LgrId
left join KrishnaiERP..[State] s on s.StateId = m.StateId
left join KrishnaiERP..Districts ds on ds.DistrictId = m.DistrictId
left join KrishnaiERP..Citys c on c.CityId = m.CityId

GO