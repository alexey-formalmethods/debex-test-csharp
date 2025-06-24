-- Faster version using INSERT with SELECT and generate_series
-- This approach is much faster for bulk inserts

WITH user_data AS (
    SELECT "Id" as user_id FROM "Users"
),
product_data AS (
    SELECT "Id" as entity_id, 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'::uuid as entity_type_id FROM "Products"
),
service_data AS (
    SELECT "Id" as entity_id, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'::uuid as entity_type_id FROM "Services"
),
action_types AS (
    SELECT "Id" as action_type_id FROM "ActionTypes"
),
entities AS (
    SELECT entity_id, entity_type_id FROM product_data
    UNION ALL
    SELECT entity_id, entity_type_id FROM service_data
),
random_data AS (
    SELECT 
        gen_random_uuid() as id,
        NOW() - INTERVAL '1 day' * floor(random() * 90) as created_at,
        (SELECT user_id FROM user_data ORDER BY random() LIMIT 1) as user_id,
        (SELECT action_type_id FROM action_types ORDER BY random() LIMIT 1) as action_type_id,
        gen_random_uuid() as status_id,
        e.entity_id,
        e.entity_type_id,
        (1 + floor(random() * 50))::integer as credits
    FROM generate_series(1, 1000000) gs
    CROSS JOIN LATERAL (
        SELECT entity_id, entity_type_id 
        FROM entities 
        ORDER BY random() 
        LIMIT 1
    ) e
)
INSERT INTO "UserActions" (
    "Id",
    "CreatedAt",
    "UserId", 
    "ActionTypeId",
    "StatusId",
    "EntityId",
    "EntityTypeId",
    "Credits"
)
SELECT 
    id,
    created_at,
    user_id,
    action_type_id,
    status_id,
    entity_id,
    entity_type_id,
    credits
FROM random_data;