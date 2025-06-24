-- Script to generate 1,000,000 UserAction records
-- Run this after the database is seeded with initial data

DO $$
DECLARE
    -- Arrays to store IDs from existing data
    user_ids uuid[];
    product_ids uuid[];
    service_ids uuid[];
    action_type_ids uuid[];
    entity_type_product_id uuid;
    entity_type_service_id uuid;
    
    -- Loop variables
    i INTEGER;
    random_user_id uuid;
    random_entity_id uuid;
    random_action_type_id uuid;
    random_entity_type_id uuid;
    random_date timestamp;
    random_credits integer;
    
BEGIN
    -- Get all existing user IDs
    SELECT array_agg("Id") INTO user_ids FROM "Users";
    
    -- Get all existing product IDs  
    SELECT array_agg("Id") INTO product_ids FROM "Products";
    
    -- Get all existing service IDs
    SELECT array_agg("Id") INTO service_ids FROM "Services";
    
    -- Get all action type IDs
    SELECT array_agg("Id") INTO action_type_ids FROM "ActionTypes";
    
    -- Get entity type IDs
    SELECT "Id" INTO entity_type_product_id FROM "EntityTypes" WHERE "Name" = 'Product';
    SELECT "Id" INTO entity_type_service_id FROM "EntityTypes" WHERE "Name" = 'Service';
    
    -- Generate 1,000,000 records in batches
    FOR i IN 1..1000000 LOOP
        -- Random user
        random_user_id := user_ids[1 + floor(random() * array_length(user_ids, 1))];
        
        -- Random action type
        random_action_type_id := action_type_ids[1 + floor(random() * array_length(action_type_ids, 1))];
        
        -- Random entity type and corresponding entity
        IF random() < 0.7 THEN -- 70% products, 30% services
            random_entity_type_id := entity_type_product_id;
            random_entity_id := product_ids[1 + floor(random() * array_length(product_ids, 1))];
        ELSE
            random_entity_type_id := entity_type_service_id;
            random_entity_id := service_ids[1 + floor(random() * array_length(service_ids, 1))];
        END IF;
        
        -- Random date within last 90 days
        random_date := NOW() - INTERVAL '1 day' * floor(random() * 90);
        
        -- Random credits between 1 and 50
        random_credits := 1 + floor(random() * 50);
        
        -- Insert the record
        INSERT INTO "UserActions" (
            "Id",
            "CreatedAt", 
            "UserId",
            "ActionTypeId",
            "StatusId",
            "EntityId",
            "EntityTypeId",
            "Credits"
        ) VALUES (
            gen_random_uuid(),
            random_date,
            random_user_id,
            random_action_type_id,
            gen_random_uuid(), -- Random status ID
            random_entity_id,
            random_entity_type_id,
            random_credits
        );
        
        -- Progress indicator every 100k records
        IF i % 100000 = 0 THEN
            RAISE NOTICE 'Inserted % records', i;
        END IF;
        
    END LOOP;
    
    RAISE NOTICE 'Successfully generated 1,000,000 UserAction records!';
    
END $$;