DROP TABLE Usersdata
CREATE TABLE Usersdata
(
	ID INT IDENTITY(1,1),
	RoleID INT,
	RoleName VARCHAR(50),
	Name VARCHAR(40),
	Email VARCHAR(100),
	Password VARCHAR(100),
	IdCode VARCHAR(20),
	PRIMARY KEY (ID)
)
INSERT INTO Usersdata(RoleID, Name, Email, Password, IdCode) VALUES 
(1, 'Admin','maximkirichenk0.06@gmail.com','12844752','NDqgXdNgLqeTflRYlylP'),
(2, 'Maxim','hatemtbofferskin@gmail.com','root','BrVLFLCDEhrCCSRyTMbD'),
(3, 'matvey','matvey@gmail.com','matvey','gfkXilapjJGiLkhblSHL'),
(1, 'FAQ','faqblogms@gmail.com','12844752Hen_','kplaBnqVErUTHMKFjWir'),
(3, 'msi','msi@gmail.com','msi','slBHEwxVMFXKfFtGDtnY')

UPDATE Usersdata SET RoleName = 'admin' WHERE RoleID = 1 
UPDATE Usersdata SET RoleName = 'moderator' WHERE RoleID = 2 
UPDATE Usersdata SET RoleName = 'user' WHERE RoleID = 3

DROP TABLE Usersroles
CREATE TABLE Usersroles
(
	ID INT IDENTITY(1,1),
	RoleID INT,
	RoleName VARCHAR(50)
)
INSERT INTO Usersroles(RoleID, RoleName) VALUES
(1, 'admin'),
(2, 'moderator'),
(3, 'user')

DROP TABLE Messagedata
CREATE TABLE Messagedata
(
	ID INT IDENTITY(1,1),
	Name VARCHAR(20),
	Email VARCHAR(100),
	Message VARCHAR(500),
	ToEmail VARCHAR(100),
	PRIMARY KEY (ID)
)
INSERT INTO Messagedata(Name, Email, Message, ToEmail) VALUES
('Maxim','hatemtbofferskin@gmail.com','hello to admin!','maximkirichenk0.06@gmail.com')


DROP TABLE Notes
CREATE TABLE Notes
(
	ID INT IDENTITY(1,1),
	IdNote UNIQUEIDENTIFIER,
	IdCode VARCHAR(20),
	Title TEXT,
	Body TEXT
	PRIMARY KEY (ID)
)
INSERT INTO Notes(IdNote, IdCode, Title, Body) VALUES
('EA157230-5202-421E-95B5-D52C875DE833','BrVLFLCDEhrCCSRyTMbD', 'First note', 'It`s the first note!'),
('F5AD0E5C-17B0-42A0-920A-9A2442A23E51','BrVLFLCDEhrCCSRyTMbD', 'commit', 'commit to branch master'),
('BB4929D2-29FE-4E2C-9EB9-3901909675EA','slBHEwxVMFXKfFtGDtnY', 'First note', 'first note from msi'),
('49896593-D1F4-4632-A06F-7293D5D1AEB0','slBHEwxVMFXKfFtGDtnY', 'Second note', 'second note from msi'),
('BDD225C9-F419-4B29-BB1F-8BD387CFC073','slBHEwxVMFXKfFtGDtnY', 'Second note', 'second note from msi'),
('7AE58CAD-27D4-44CA-AB54-2909BF9541DD','slBHEwxVMFXKfFtGDtnY', 'handler', 'need to add note handler in project'),
('00889CBA-1669-4A8B-B0E8-3440F0A12830','slBHEwxVMFXKfFtGDtnY', 'commit', 'then i have to commit it')

SELECT * FROM Notes