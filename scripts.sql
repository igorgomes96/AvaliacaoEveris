CREATE DATABASE Produtos

CREATE TABLE Empresas(
	Id int IDENTITY(1,1) PRIMARY KEY,
	Nome nvarchar(255) NOT NULL
) 

CREATE TABLE Produtos(
	Id int NOT NULL PRIMARY KEY,
	Nome nvarchar(255) NOT NULL,
)


CREATE TABLE Estoque(
	Id int IDENTITY(1,1) PRIMARY KEY,
	ProdutoId int NOT NULL,
	EmpresaId int NOT NULL,
	Data datetime2(7) NOT NULL,
	Entrada int NOT NULL,
	Saida int NOT NULL,
	Qtda int NOT NULL,
	CONSTRAINT FK_Estoque_Empresas_EmpresaId FOREIGN KEY(EmpresaId) REFERENCES Empresas (Id) ON DELETE CASCADE,
	CONSTRAINT FK_Estoque_Produtos_ProdutoId FOREIGN KEY(ProdutoId) REFERENCES Produtos (Id) ON DELETE CASCADE
)

CREATE UNIQUE INDEX IX_Empresas_Nome ON Empresas (Nome)
CREATE UNIQUE INDEX IX_Estoque_Data_ProdutoId_EmpresaId ON Estoque
(
	Data,
	ProdutoId,
	EmpresaId
)
CREATE INDEX IX_Estoque_EmpresaId ON Estoque (EmpresaId)
CREATE INDEX IX_Estoque_ProdutoId ON Estoque (ProdutoId)
CREATE UNIQUE INDEX IX_Produtos_Nome ON Produtos (Nome)
