USE master;
GO

-- create database if not exists 
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'machine_db')
BEGIN
    CREATE DATABASE machine_db;
END
GO

USE machine_db;
GO


DROP TABLE IF EXISTS work_logs;
DROP TABLE IF EXISTS machines;
DROP TABLE IF EXISTS projects;
DROP TABLE IF EXISTS operators;
DROP TABLE IF EXISTS machine_types;
GO

CREATE TABLE machine_types (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    maintenance_interval_hours INT,
    created_at DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE operators (
    id INT PRIMARY KEY IDENTITY(1,1),
    first_name NVARCHAR(50) NOT NULL,
    last_name NVARCHAR(50) NOT NULL,
    badge_number NVARCHAR(20) UNIQUE NOT NULL,
    email NVARCHAR(100),
    is_active BIT DEFAULT 1
);

CREATE TABLE projects (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    client_name NVARCHAR(100),
    deadline DATE,
    status NVARCHAR(20)
);

CREATE TABLE machines (
    id INT PRIMARY KEY IDENTITY(1,1),
    code NVARCHAR(20) UNIQUE NOT NULL,
    machine_type_id INT,
    status NVARCHAR(20),
    location NVARCHAR(50),
    purchase_date DATE,
    CONSTRAINT FK_Machines_Types FOREIGN KEY (machine_type_id) REFERENCES machine_types(id)
);

CREATE TABLE work_logs (
    id INT PRIMARY KEY IDENTITY(1,1),
    machine_id INT NOT NULL,
    operator_id INT NOT NULL,
    project_id INT NOT NULL,
    start_time DATETIME2 NOT NULL,
    end_time DATETIME2,
    output_quantity INT,
    notes NVARCHAR(MAX),
    CONSTRAINT FK_Logs_Machines FOREIGN KEY (machine_id) REFERENCES machines(id),
    CONSTRAINT FK_Logs_Operators FOREIGN KEY (operator_id) REFERENCES operators(id),
    CONSTRAINT FK_Logs_Projects FOREIGN KEY (project_id) REFERENCES projects(id)
);
GO