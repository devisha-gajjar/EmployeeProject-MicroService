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


