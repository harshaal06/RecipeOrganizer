CREATE DATABASE RecipeOrganizerDB;
USE RecipeOrganizerDB;

CREATE TABLE Users (
	Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EntityId CHAR(36) NOT NULL UNIQUE,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    UserName VARCHAR(100) NOT NULL UNIQUE,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    IsEmailVerified BOOLEAN NOT NULL DEFAULT FALSE,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL
);

CREATE TABLE Roles (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EntityId CHAR(36) NOT NULL UNIQUE,
    Name VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255),
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(100) NULL
);

CREATE TABLE UserRoles (
	Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    RoleId CHAR(36) NOT NULL,

    PRIMARY KEY (UserId, RoleId),

    CONSTRAINT FK_UserRoles_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_UserRoles_Roles
        FOREIGN KEY (RoleId)
        REFERENCES Roles(Id)
        ON DELETE CASCADE
);

INSERT INTO Roles (Id, Name, Description)
VALUES
(UUID(), 'Admin', 'System Administrator'),
(UUID(), 'User', 'Application User');

ALTER TABLE UserRoles
ADD COLUMN IsActive BOOLEAN NOT NULL DEFAULT TRUE;

SELECT * FROM Roles;
SELECT * FROM Users;
SELECT * FROM UserRoles;

Drop Table roles;
DROP TABLE Users;

DELETE FROM UserRoles;
 
 where UserId = '' and roleId = '';

update UserRoles set RoleId = 'd592d0e4-6364-11f1-a458-b80b0e248c50';
DELETE FROM Users;

Update Users
set IsActive =1, Email = 'harshalaglawe1@gmail.com' 
where UserName = 'harshaal';

ALTER TABLE UserRoles
ADD COLUMN Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY;

ALTER TABLE UserRoles
DROP PRIMARY KEY;


SELECT u.Id, u.FirstName, u.LastName, u.UserName, u.Email, u.PasswordHash, r.Name AS Role 
FROM Users u 
INNER JOIN UserRoles ur ON u.Id = ur.UserId 
INNER JOIN Roles r ON ur.RoleId = r.Id 
WHERE u.IsActive = 1 and ( u.Email = 'harshalaglawe1@gmail.com' OR u.UserName = 'harshaal');

select * from Users
where email = 'harshalaglawe1@gmail.com';

SELECT u.Id AS UserId, r.Id AS RoleId FROM Users u 
CROSS JOIN Roles r 
WHERE u.UserName = 'harshaal' AND r.Name = 'Admin' AND u.IsActive = 1 AND r.IsActive = 1;

SELECT u.Id, u.FirstName, u.LastName, u.UserName, u.Email, GROUP_CONCAT(r.Name) AS Roles 
FROM Users u 
LEFT JOIN UserRoles ur ON u.Id = ur.UserId LEFT JOIN Roles r ON ur.RoleId = r.Id 
WHERE u.UserName IN ('harshaal') and u.IsActive = 1 
GROUP BY u.Id, u.FirstName, u.LastName, u.UserName, u.Email