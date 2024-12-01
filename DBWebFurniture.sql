CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    Name NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    CreatedDate          datetime             null,
   CreatedBy            varchar(255)         null,
   UpdatedDate          datetime             null,
   UpdatedBy            varchar(255)         null,
);


CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    Name NVARCHAR(20) NOT NULL UNIQUE,
    CreatedDate          datetime             null,
   CreatedBy            varchar(255)         null,
   UpdatedDate          datetime             null,
   UpdatedBy            varchar(255)         null,
);


CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL, 
    Price money NOT NULL,
    Stock INT NOT NULL,
    Image NVARCHAR(255),
    Category_id INT,
    CreatedDate          datetime             null,
   CreatedBy            varchar(255)         null,
   UpdatedDate          datetime             null,
   UpdatedBy            varchar(255)         null,
    FOREIGN KEY (Category_id) REFERENCES Categories(Id) ON DELETE SET NULL
);


CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    User_id INT NOT NULL,
    Total_price money NULL,
    Status int null,
    CreatedDate          datetime             null,
   CreatedBy            varchar(255)         null,
   UpdatedDate          datetime             null,
   UpdatedBy            varchar(255)         null,
    FOREIGN KEY (User_id) REFERENCES Users(Id) ON DELETE CASCADE
);


CREATE TABLE Order_Details (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Order_id INT NOT NULL,
    Product_id INT NOT NULL,
    Quantity INT NOT NULL,
    Price money NOT NULL,
    CreatedDate          datetime             null,
   CreatedBy            varchar(255)         null,
   UpdatedDate          datetime             null,
   UpdatedBy            varchar(255)         null,
    FOREIGN KEY (Order_id) REFERENCES Orders(Id) ON DELETE CASCADE,
    FOREIGN KEY (Product_id) REFERENCES Products(Id) ON DELETE CASCADE
);


CREATE TABLE Reviews (
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    Product_id INT NOT NULL,
    User_id INT NOT NULL,
    Rating INT NOT NULL,
    Comment TEXT,
    CreatedDate          datetime             null,
   CreatedBy            varchar(255)         null,
   UpdatedDate          datetime             null,
   UpdatedBy            varchar(255)         null,
    FOREIGN KEY (Product_id) REFERENCES Products(Id) ON DELETE CASCADE,
    FOREIGN KEY (User_id) REFERENCES Users(Id) ON DELETE CASCADE
);


CREATE TABLE Carts (
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    User_id INT NOT NULL,
    Product_id INT NOT NULL,
    Quantity INT NOT NULL,
    CreatedDate          datetime             null,
   CreatedBy            varchar(255)         null,
   UpdatedDate          datetime             null,
   UpdatedBy            varchar(255)         null,
    FOREIGN KEY (User_id) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (Product_id) REFERENCES Products(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_UserProduct UNIQUE (User_id, Product_id) -- Đảm bảo mỗi sản phẩm chỉ xuất hiện một lần trong giỏ hàng của người dùng
);


CREATE TABLE Pages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(255) NOT NULL UNIQUE,
    Content TEXT NOT NULL,
    CreatedDate          datetime             null,
   CreatedBy            varchar(255)         null,
   UpdatedDate          datetime             null,
   UpdatedBy            varchar(255)         null,
);

