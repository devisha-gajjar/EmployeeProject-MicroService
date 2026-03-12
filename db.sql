INSERT INTO tenant1.employee_list (name, email, department_id, salary)
VALUES 
    ('Jane Smith', 'jane.smith@company.com', 1, 62000.50),
    ('Mike Ross', 'mike.ross@legal.com', 2, 48000.00),
    ('Rachel Zane', 'rachel.zane@hr.com', 2, 52000.00),
    ('Harvey Specter', 'harvey.s@suits.com', 3, 95000.00),
    ('Donna Paulsen', 'donna.p@exec.com', 3, 75000.00);



INSERT INTO tenant1.departments (name, manager_id)
VALUES 
    ('Human Resources', NULL),
    ('Information Technology', NULL),
    ('Finance', NULL),
    ('Sales', NULL);



-- Create the Master Tenants Table
CREATE TABLE IF NOT EXISTS public.tenants
(
    tenant_id SERIAL PRIMARY KEY,
    company_name character varying(255) NOT NULL,
    schema_name character varying(63) UNIQUE NOT NULL, -- e.g., 'tenant_acme'
    is_active boolean DEFAULT true,
    created_on timestamp DEFAULT CURRENT_TIMESTAMP
);

-- Add 2 Dummy Tenants
INSERT INTO public.tenants (company_name, schema_name)
VALUES 
('Acme Corp', 'tenant_acme'),
('Globex IT', 'tenant_globex');


ALTER TABLE public.users 
ADD COLUMN tenant_id integer;

-- 2. Create the Foreign Key constraint
ALTER TABLE public.users
ADD CONSTRAINT fk_tenant
FOREIGN KEY (tenant_id) 
REFERENCES public.tenants (tenant_id)
ON DELETE CASCADE;


-- 3. (Optional) Assign a default tenant to existing users
-- If you have an existing tenant with ID 1, run this:
UPDATE public.users SET tenant_id = 1 WHERE tenant_id IS NULL;

-- 4. (Optional) Make it NOT NULL after assigning data
ALTER TABLE public.users ALTER COLUMN tenant_id SET NOT NULL;