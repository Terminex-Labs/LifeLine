CREATE OR REPLACE VIEW "V_Employee_Full_Details" AS
SELECT
    e."Id" AS "EmployeeId",
    e."Surname",
    e."Name",
    e."Patronymic",
    e."DateEntry",
    e."Rating",
    e."PersonalPhoto",
    e."IsActive",
    
    jsonb_build_object(
        'GenderId', g."Id",
        'GenderName', g."Name"
    ) AS "Gender",

    (SELECT jsonb_build_object(
            'ContactInformationId', ci."Id",
            'PersonalPhone', ci."PersonalPhone",
            'CorporatePhone', ci."CorporatePhone",
            'PersonalEmail', ci."PersonalEmail",
            'CorporateEmail', ci."CorporateEmail",
            'PostalCode', ci."PostalCode",
            'Region', ci."Region",
            'City', ci."City",
            'Street', ci."Street",
            'Building', ci."Building",
            'Apartment', ci."Apartment"
           ) 
     FROM "ContactInformations" ci WHERE ci."EmployeeId" = e."Id"
    ) AS "ContactInformation",

    (SELECT COALESCE(jsonb_agg(jsonb_build_object(
                'AssignmentId', a."Id",
                'PositionId', a."PositionId",
                'DepartmentId', a."DepartmentId",
                'ManagerId', a."ManagerId",
                'HireDate', a."HireDate",
                'TerminationDate', a."TerminationDate",
                'StatusId', a."StatusId",
                'ContractId', a."ContractId"
            )), '[]'::jsonb) 
     FROM "Assignments" a WHERE a."EmployeeId" = e."Id"
    ) AS "Assignments",

    (SELECT COALESCE(jsonb_agg(jsonb_build_object(
                'ContractId', c."Id",
                'ContractNumber', c."ContractNumber",
                'EmployeeTypeId', et."Id",
                'ContractStartDate', c."StartDate",
                'ContractEndDate', c."EndDate",
                'Salary', c."Salary",
                'ContractFileKey', c."FileKey"
            )), '[]'::jsonb) 
     FROM "Contracts" c 
     JOIN "EmployeeTypes" et ON c."EmployeeTypeId" = et."Id" 
     WHERE c."EmployeeId" = e."Id"
    ) AS "Contracts",

    (SELECT COALESCE(jsonb_agg(jsonb_build_object(
                'EducationDocumentId', ed."Id",
                'EducationLevelId', ed."EducationLevelId",
                'EducationDocumentTypeId', ed."DocumentTypeId",
                'EducationDocumentNumber', ed."DocumentNumber",
                'EducationIssuedDate', ed."IssuedDate",
                'OrganizationName', ed."OrganizationName",
                'QualificationAwardedName', ed."QualificationAwardedName",
                'EducationSpecialtyName', ed."SpecialtyName",
                'ProgramName', ed."ProgramName",
                'TotalHours', ed."TotalHours"
           )), '[]'::jsonb) 
     FROM "EducationDocuments" ed WHERE ed."EmployeeId" = e."Id"
    ) AS "EducationDocuments",

    (SELECT COALESCE(jsonb_agg(jsonb_build_object(
                'PersonalDocumentId', pd."Id",
                'PersonalDocumentTypeId', pd."DocumentTypeId",
                'PersonalDocumentNumber', pd."Number",
                'PersonalDocumentSeries', pd."Series",
                'PersonalDocumentFileKey', pd."FileKey"
           )), '[]'::jsonb) 
     FROM "PersonalDocuments" pd WHERE pd."EmployeeId" = e."Id"
    ) AS "PersonalDocuments",

    (SELECT COALESCE(jsonb_agg(jsonb_build_object(
                'SpecialtyId', s."Id",
                'SpecialtyName', s."Name",
                'SpecialtyDescription', s."Description"
           )), '[]'::jsonb)
     FROM "EmployeeSpecialties" es
     JOIN "Specialties" s ON es."SpecialtyId" = s."Id"
     WHERE es."EmployeeId" = e."Id"
    ) AS "Specialties",

    (SELECT COALESCE(jsonb_agg(jsonb_build_object(
                'WorkPermitId', wp."Id",
                'WorkPermitName', wp."WorkPermitName",
                'WorkPermitDocumentSeries', wp."DocumentSeries",
                'WorkPermitNumber', wp."WorkPermitNumber",
                'ProtocolNumber', wp."ProtocolNumber",
                'WorkPermitSpecialtyName', wp."SpecialtyName",
                'IssuingAuthority', wp."IssuingAuthority",
                'WorkPermitIssueDate', wp."IssueDate",
                'WorkPermitExpiryDate', wp."ExpiryDate",
                'WorkPermitFileKey', wp."FileKey",
                'PermitTypeId', wp."PermitTypeId",
                'AdmissionStatusId', wp."AdmissionStatusId"
           )), '[]'::jsonb)
     FROM "WorkPermits" wp WHERE wp."EmployeeId" = e."Id"
    ) AS "WorkPermits"

FROM
    "Employees" e
LEFT JOIN
    "Genders" g ON e."GenderId" = g."Id"
GROUP BY
    e."Id", g."Id";