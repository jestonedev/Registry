INSERT INTO registry.rent_types
SELECT
  id_Rent, RentType, RentTypeShort, RentType_Rod
FROM socnaim.sp_renttype;

INSERT INTO executors
SELECT
  id_Executor, ExecutorName, '' AS login, 0
FROM socnaim.sp_executors;

INSERT INTO registry.warrant_doc_types
SELECT id_Doc, CaptionSingle
FROM socnaim.sp_doc;

INSERT INTO warrants
SELECT w.id_Warrant, w.id_Doc, w.RegNum,
                               w.RegDate,
                               w.OnBehalfOf,
                               w.NotaryFIO,
                               w.NotarylDistrict,
                               w.Note, 0
FROM socnaim.warrants w
WHERE w.RegDate IS NOT NULL;

INSERT INTO registry.tenancy_contracts
SELECT
  id_Contract, 
  id_RentType, 
  id_Warrant, 
  ContractRegistrationNumber, 
  ContractRegistrationDate, 
  ContractIssueDate, 
  ContractBeginDate, 
  ContractEndDate, 
  isContractPermanent, 
  NULL AS residence_warrant_num,
  NULL AS residence_warrant_date, 
  NULL AS kumi_order_num,
  NULL AS kumi_order_date,
  id_Executor, 
  note, 
  0
FROM socnaim.contracts

UPDATE persons
set id_document_issued_by = 
(
SELECT id_document_issued_by 
FROM document_issued_by d
WHERE d.document_issued_by =
(SELECT
  `PassportIssued`
FROM socnaim.persons xx
WHERE (`PassportIssued` REGEXP 'УВД' OR
  `PassportIssued` REGEXP 'ОВД' OR
  `PassportIssued` REGEXP 'ОМ' OR
  `PassportIssued` REGEXP 'МВД' OR
  `PassportIssued` REGEXP 'УФМС' OR
  `PassportIssued` REGEXP 'милиции' OR
  `PassportIssued` REGEXP 'ЗАГС' OR
  `PassportIssued` REGEXP 'Управление' OR
  `PassportIssued` REGEXP 'Министерство юстиции республика Казахстан' OR
  `PassportIssued` REGEXP 'отдел по г. Братску /Центральный р-он в управлении гос. регистрации службы' AND
  `PassportIssued` REGEXP 'Администрация п. Порожский г. Братска Иркутской обл.' OR 
  `PassportIssued` REGEXP 'Отдел внутренних дел Куйбышевского района города Иркутска') AND
   xx.id_Person = persons.id_person))