USE machine_db;
GO

-- =============================================
-- 1. POPULATE MACHINE TYPES
-- =============================================
INSERT INTO machine_types (name, maintenance_interval_hours) VALUES 
('3D Printer (FDM)', 500),
('3D Printer (SLA)', 300),
('CNC Mill (3-Axis)', 1000),
('CNC Mill (5-Axis)', 800),
('Laser Cutter', 400),
('Injection Molder', 2000),
('Soldering Station', 100);

-- =============================================
-- 2. POPULATE OPERATORS
-- =============================================
INSERT INTO operators (first_name, last_name, badge_number, email, is_active) VALUES 
('John', 'Doe', 'OP-1001', 'john.d@factory.com', 1),
('Sarah', 'Connor', 'OP-1002', 'sarah.c@factory.com', 1),
('Mike', 'Ross', 'OP-1003', 'mike.r@factory.com', 0),
('Elena', 'Rodriguez', 'OP-1004', 'elena.r@factory.com', 1),
('David', 'Chen', 'OP-1005', 'david.c@factory.com', 1),
('Fiona', 'Gallagher', 'OP-1006', 'fiona.g@factory.com', 1),
('George', 'Miller', 'OP-1007', 'george.m@factory.com', 0),
('Hannah', 'Baker', 'OP-1008', 'hannah.b@factory.com', 1);

-- =============================================
-- 3. POPULATE PROJECTS
-- =============================================
INSERT INTO projects (name, client_name, deadline, status) VALUES 
('Project Alpha-1', 'Tesla Inc', '2023-12-01', 'completed'),
('Gearbox Assembly', 'Ford', '2024-06-15', 'in_progress'),
('Custom Engraving', 'Etsy Bulk', '2024-02-20', 'completed'),
('Drone Chassis', 'DJI', '2024-08-01', 'in_progress'),
('Medical Stents', 'Medtronic', '2024-05-10', 'planned'),
('Prototype Housing', 'Apple', '2024-01-15', 'completed'),
('Suspension Arms', 'Rivian', '2024-09-30', 'in_progress'),
('Plastic Toys', 'Hasbro', '2024-04-01', 'planned'),
('Circuit Boards', 'Nvidia', '2024-07-20', 'in_progress'),
('Spare Parts B', 'Internal', '2025-01-01', 'planned');

-- =============================================
-- 4. POPULATE MACHINES
-- =============================================
INSERT INTO machines (code, machine_type_id, status, location, purchase_date) VALUES 
('PRT-001', 1, 'active', 'Room A', '2022-01-10'),
('PRT-002', 1, 'maintenance', 'Room A', '2022-03-15'),
('SLA-001', 2, 'active', 'Lab 1', '2023-01-05'),
('CNC-3A-01', 3, 'active', 'Floor 1', '2020-11-05'),
('CNC-3A-02', 3, 'retired', 'Warehouse', '2018-06-20'),
('CNC-5A-01', 4, 'active', 'Floor 1', '2021-08-12'),
('LSR-X1', 5, 'active', 'Room C', '2023-05-20'),
('LSR-X2', 5, 'maintenance', 'Room C', '2023-06-01'),
('INJ-500', 6, 'active', 'Floor 2', '2019-12-01'),
('INJ-600', 6, 'active', 'Floor 2', '2020-02-15'),
('SOL-01', 7, 'active', 'Bench 1', '2023-09-01'),
('SOL-02', 7, 'active', 'Bench 2', '2023-09-01');

-- =============================================
-- 5. POPULATE WORK LOGS (Rich history)
-- =============================================
INSERT INTO work_logs (machine_id, operator_id, project_id, start_time, end_time, output_quantity, notes) VALUES 
-- Completed jobs from the past
(4, 1, 2, '2024-01-10 08:00:00', '2024-01-10 12:00:00', 50, 'Smooth operation, no issues'),
(1, 2, 6, '2024-01-11 09:30:00', '2024-01-11 14:45:00', 1, 'Filament change required halfway'),
(4, 4, 2, '2024-01-12 08:00:00', '2024-01-12 16:00:00', 120, 'High output shift'),
(7, 5, 3, '2024-01-13 10:00:00', '2024-01-13 11:30:00', 200, 'Batch engraving complete'),
(6, 1, 4, '2024-01-14 07:00:00', '2024-01-14 19:00:00', 15, 'Complex milling, slow feed rate'),
(9, 6, 8, '2024-01-15 08:00:00', '2024-01-15 12:00:00', 500, 'Mold temp fluctuated initially'),
(9, 6, 8, '2024-01-15 13:00:00', '2024-01-15 17:00:00', 550, 'Stabilized process'),

-- Recent jobs
(11, 8, 9, '2024-02-01 09:00:00', '2024-02-01 12:00:00', 30, 'Hand soldering prototype board'),
(3, 2, 6, '2024-02-02 14:00:00', '2024-02-02 18:00:00', 2, 'Resin tank refill needed'),
(10, 5, 7, '2024-02-03 06:00:00', '2024-02-03 14:00:00', 80, 'Standard run'),
(4, 4, 2, '2024-02-04 08:00:00', '2024-02-04 12:00:00', 45, 'Tool breakage, downtime 30 mins'),

-- Maintenance / Issues
(2, 1, 10, '2024-02-05 08:00:00', '2024-02-05 09:00:00', 0, 'Calibration run only - failed'),
(2, 1, 10, '2024-02-05 09:30:00', '2024-02-05 10:00:00', 0, 'Maintenance check performed'),

-- Varied operators on same machine
(6, 4, 4, '2024-02-06 08:00:00', '2024-02-06 16:00:00', 10, 'Day shift progress'),
(6, 5, 4, '2024-02-06 16:00:00', '2024-02-07 00:00:00', 12, 'Night shift continuation'),

-- Currently Running Jobs (end_time IS NULL)
(1, 2, 10, '2024-02-08 08:30:00', NULL, 0, 'Started morning print job'),
(7, 8, 3, '2024-02-08 09:15:00', NULL, 0, 'Engraving batch 55'),
(4, 1, 2, '2024-02-08 10:00:00', NULL, 0, 'Setup for new gear run'),
(11, 6, 9, '2024-02-08 11:00:00', NULL, 0, 'Reworking faulty connections');
GO