CREATE OR REPLACE VIEW "V_Employee_Hr_Items" AS
    SELECT 
        e."Id",
        e."Surname",
        e."Name",
        e."Patronymic",
        e."PersonalPhoto",
        e."IsActive",

        COALESCE(
            jsonb_agg(
                jsonb_build_object(
                    'DepartmentId', a."DepartmentId",
                    'PositionId',   a."PositionId",
                    'StatusId',     a."StatusId"
                )
            ) 
            FILTER (WHERE a."Id" IS NOT NULL), 
            '[]'::jsonb
        ) AS "Assignments"

    FROM "Employees" AS e
    LEFT JOIN "Assignments" AS a ON e."Id" = a."EmployeeId"
    GROUP BY 
        e."Id";