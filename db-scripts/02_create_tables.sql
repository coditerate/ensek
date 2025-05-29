USE [Ensek]
GO

-- DELETE ALL TABLES IF EXISTING
IF OBJECT_ID('Account') IS NOT NULL DROP TABLE Borehole
GO

IF OBJECT_ID('MeterReading') IS NOT NULL DROP TABLE BoreholeToolType
GO


-- CREATE LOOKUP TABLES
CREATE TABLE Account (	AccountId	INT IDENTITY PRIMARY KEY,
						FirstName	VARCHAR(50) NOT NULL,
						LastName	VARCHAR(50) NOT NULL)
GO

CREATE TABLE MeterReading (	MeterReadingId			INT IDENTITY PRIMARY KEY,
							AccountId				INT NOT NULL,
							MeterReadingDateTime	DATETIME NOT NULL,
							MeterReadValue			INT NOT NULL,
							FOREIGN KEY (AccountId) REFERENCES Account(AccountId))
GO
