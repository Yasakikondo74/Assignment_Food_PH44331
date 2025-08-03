create database FoodDB
go
use FoodDB
go

create table Food(
    ID char(50) primary key not null,
    Food_Name nvarchar(50) unique,
    Available bit,
    Quantity int,
    Cost money,
    Image_Link nvarchar(500)
)

create table Account(
    ID char(50) primary key not null,
    acc_username nvarchar(50) unique not null,
    acc_password nvarchar(50) not null,
    acc_role char(10)
)

create table Receipt(
    ID char(50) primary key not null,
    Account_ID char(50) foreign key references Account(ID) not null,
    CreatedAt datetime default getdate()
)

create table ReceiptDetail(
    ID char(50) primary key not null,
    Receipt_ID char(50) foreign key references Receipt(ID) not null,
    Food_ID char(50) foreign key references Food(ID) not null,
    Quantity int,
    TotalCost money
)

-- Insert data into Food
insert into Food values
('F001', 'Burger', 1, 50, 5.00, 'https://stories.inspirebrands.com/wp-content/uploads/2023/04/AllAmericanBurger_MF12.jpg'),
('F002', 'Pizza', 1, 30, 8.50, 'https://cdn.britannica.com/08/177308-050-94D9D6BE/Food-Pizza-Basil-Tomato.jpg'),
('F003', 'Soda', 1, 100, 1.50, 'http://walkingoffpounds.com/wp-content/uploads/2017/05/Soda-Cans.jpg'),
('F004', 'Salad', 1, 25, 4.00, 'https://i0.wp.com/resiliencefitness.com/wp-content/uploads/2013/09/salad.jpg')

-- Insert data into Account
insert into Account values
('A001', 'john_doe', 'password123', 'customer'),
('A002', 'jane_smith', 'securepass', 'customer'),
('A003', 'admin', '123', 'admin'),
('A004', 'user', '123', 'customer')

-- Insert data into Receipt
insert into Receipt values
('R001', 'A001', getdate()),
('R002', 'A002', getdate())

-- Insert data into ReceiptDetail
insert into ReceiptDetail values
('RD001', 'R001', 'F001', 2, 10.00),  -- 2 Burgers
('RD002', 'R001', 'F003', 1, 1.50),  -- 1 Soda
('RD003', 'R002', 'F002', 1, 8.50),  -- 1 Pizza
('RD004', 'R002', 'F004', 2, 8.00)   -- 2 Salads
