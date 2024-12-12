-- Table Banners
CREATE TABLE [Banners] (
    [BAN_ID] INT IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(MAX) NOT NULL,
    [Image] NVARCHAR(MAX) NOT NULL,
    [DisplayOrder] INT NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(MAX) NOT NULL,
    [UpdatedDate] DATETIME NULL,
    [UpdatedBy] NVARCHAR(MAX) NULL
);

-- Table Features
CREATE TABLE [Features] (
    [FEA_ID] INT IDENTITY(1,1) PRIMARY KEY,
    [Icon] NVARCHAR(MAX) NOT NULL,
    [Title] NVARCHAR(MAX) NOT NULL,
    [Subtitle] NVARCHAR(MAX) NOT NULL,
    [DisplayOrder] INT NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(MAX) NOT NULL,
    [UpdatedDate] DATETIME NULL,
    [UpdatedBy] NVARCHAR(MAX) NULL
);

-- Table Settings
CREATE TABLE [Settings] (
    [SET_ID] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(MAX) NOT NULL,
    [UpdatedDate] DATETIME NULL,
    [UpdatedBy] NVARCHAR(MAX) NULL
);

-- Table AdminUsers
CREATE TABLE [AdminUsers] (
    [USE_ID] INT IDENTITY(1,1) PRIMARY KEY,
    [Username] NVARCHAR(MAX) NOT NULL,
    [Password] NVARCHAR(MAX) NOT NULL,
    [DisplayOrder] INT NOT NULL,
    [Email] NVARCHAR(MAX) NOT NULL,
    [Phone] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(MAX) NOT NULL,
    [UpdatedDate] DATETIME NULL,
    [UpdatedBy] NVARCHAR(MAX) NULL
);

-- Table Pages
CREATE TABLE [Pages] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(MAX) NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(MAX) NOT NULL,
    [UpdatedDate] DATETIME NULL,
    [UpdatedBy] NVARCHAR(MAX) NULL
);

-- Table Users
CREATE TABLE [Users] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Email] NVARCHAR(MAX) NOT NULL,
    [Password] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(MAX) NOT NULL,
    [UpdatedDate] DATETIME NULL,
    [UpdatedBy] NVARCHAR(MAX) NULL
);

-- Table Categories
CREATE TABLE [Categories] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(MAX) NOT NULL,
    [UpdatedDate] DATETIME NULL,
    [UpdatedBy] NVARCHAR(MAX) NULL
);

-- Table Products
CREATE TABLE [Products] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [Price] DECIMAL(10, 2) NOT NULL,
    [Stock] INT NOT NULL,
    [Image] NVARCHAR(MAX) NOT NULL,
    [Category_id] INT NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(MAX) NOT NULL,
    [UpdatedDate] DATETIME NULL,
    [UpdatedBy] NVARCHAR(MAX) NULL,
    FOREIGN KEY ([Category_id]) REFERENCES [Categories]([Id])
);

-- Table Orders
CREATE TABLE [Orders] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [User_id] INT NOT NULL,
    [Total_price] DECIMAL(10, 2) NOT NULL,
    [Status] INT NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(MAX) NOT NULL,
    [UpdatedDate] DATETIME NULL,
    [UpdatedBy] NVARCHAR(MAX) NULL,
    FOREIGN KEY ([User_id]) REFERENCES [Users]([Id])
);

-- Table Order_Details
CREATE TABLE [Order_Details] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Order_id] INT NOT NULL,
    [Product_id] INT NOT NULL,
    [Quantity] INT NOT NULL,
    [Price] DECIMAL(10, 2) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(MAX) NOT NULL,
    [UpdatedDate] DATETIME NULL,
    [UpdatedBy] NVARCHAR(MAX) NULL,
    FOREIGN KEY ([Order_id]) REFERENCES [Orders]([Id]),
    FOREIGN KEY ([Product_id]) REFERENCES [Products]([Id])
);

-- Table Reviews
CREATE TABLE [Reviews] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Product_id] INT NOT NULL,
    [User_id] INT NOT NULL,
    [Rating] INT NOT NULL,
    [Comment] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(MAX) NOT NULL,
    [UpdatedDate] DATETIME NULL,
    [UpdatedBy] NVARCHAR(MAX) NULL,
    FOREIGN KEY ([Product_id]) REFERENCES [Products]([Id]),
    FOREIGN KEY ([User_id]) REFERENCES [Users]([Id])
);
