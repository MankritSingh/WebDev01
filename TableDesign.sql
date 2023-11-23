CREATE DATABASE UserInformation;
USE UserInformation;
CREATE TABLE UserInfo
(
   ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,--clustered means data is stored in memory based on cluster val
   UserID AS 'UID' + RIGHT('00000000' + CAST(ID AS VARCHAR(8)), 8) PERSISTED, --Persisted:Should be calc and stored
   UserName VARCHAR(50) NOT NULL,
   UserPassword VARCHAR(50) NOT NULL,
   UserPasswordHash VARCHAR(100) NOT NULL,
   UserEmailId VARCHAR(100) NOT NULL,

)