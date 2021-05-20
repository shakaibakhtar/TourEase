
/****** Object:  Database [TourEaseDb]    Script Date: 5/20/2021 6:24:31 PM ******/
CREATE DATABASE [TourEaseDb]
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TourEaseDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TourEaseDb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TourEaseDb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TourEaseDb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TourEaseDb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TourEaseDb] SET ARITHABORT OFF 
GO
ALTER DATABASE [TourEaseDb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TourEaseDb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TourEaseDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TourEaseDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TourEaseDb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TourEaseDb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TourEaseDb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TourEaseDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TourEaseDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TourEaseDb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TourEaseDb] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [TourEaseDb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TourEaseDb] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [TourEaseDb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TourEaseDb] SET  MULTI_USER 
GO
ALTER DATABASE [TourEaseDb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TourEaseDb] SET ENCRYPTION ON
GO
USE [TourEaseDb]
GO
/****** Object:  Table [dbo].[tblGuestRequests]    Script Date: 5/20/2021 6:24:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblGuestRequests](
	[GuestRequestId] [int] IDENTITY(1,1) NOT NULL,
	[HostId] [int] NULL,
	[GuestId] [int] NULL,
	[Message] [nvarchar](max) NULL,
	[IsAccepted] [bit] NULL,
	[RatingValue] [float] NULL,
 CONSTRAINT [PK_tblGuestRequests] PRIMARY KEY CLUSTERED 
(
	[GuestRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblHostRequests]    Script Date: 5/20/2021 6:24:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblHostRequests](
	[RequestId] [int] IDENTITY(1,1) NOT NULL,
	[HostId] [int] NULL,
	[GuestId] [int] NULL,
	[Message] [varchar](max) NULL,
	[IsAccepted] [bit] NULL,
	[RatingValue] [float] NULL,
 CONSTRAINT [PK_tblRequests] PRIMARY KEY CLUSTERED 
(
	[RequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUser]    Script Date: 5/20/2021 6:24:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblUser](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Full_Name] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Password] [nvarchar](50) NULL,
	[Contact_Number] [varchar](30) NULL,
	[Location_City] [nvarchar](50) NULL,
	[Location_Area] [nvarchar](50) NULL,
	[User_Type] [int] NULL,
	[Fake_Reported_Count] [int] NULL,
	[Is_Verified] [bit] NULL,
	[Is_Enabled] [bit] NULL,
 CONSTRAINT [PK_tblUser] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUserType]    Script Date: 5/20/2021 6:24:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblUserType](
	[UserTypeId] [int] IDENTITY(1,1) NOT NULL,
	[UserTypeName] [varchar](50) NULL,
 CONSTRAINT [PK_UserType] PRIMARY KEY CLUSTERED 
(
	[UserTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[tblGuestRequests] ON 
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (3, 2, 1, N'host send request to guest', 1, 3)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (4, 2, 1, N'request to all guests to attend event', 1, 4)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (5, 2, 4, N'request to all guests to attend event', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (6, 5, 1, N'BBQ ', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (7, 5, 4, N'BBQ ', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (8, 7, 6, N'hello world', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (9, 12, 4, N'hi
', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (10, 11, 1, N'hi', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (11, 11, 4, N'hi', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (12, 11, 6, N'hi', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (13, 11, 10, N'hi', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (14, 11, 20, N'hi', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (15, 2, 1, N'today ', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (16, 2, 4, N'today ', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (17, 2, 6, N'today ', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (18, 2, 10, N'today ', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (19, 2, 20, N'today ', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (20, 2, 21, N'today ', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (21, 22, 1, N'Today Is 6 PM BBQ Party', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (22, 22, 4, N'Today Is 6 PM BBQ Party', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (23, 22, 6, N'Today Is 6 PM BBQ Party', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (24, 22, 10, N'Today Is 6 PM BBQ Party', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (25, 22, 20, N'Today Is 6 PM BBQ Party', 0, 0)
GO
INSERT [dbo].[tblGuestRequests] ([GuestRequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (26, 22, 21, N'Today Is 6 PM BBQ Party', 0, 0)
GO
SET IDENTITY_INSERT [dbo].[tblGuestRequests] OFF
GO
SET IDENTITY_INSERT [dbo].[tblHostRequests] ON 
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (3, 2, 1, N'guest send request to host', 1, 4)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (4, 2, 1, N'I want to join an event if available call me', 1, 3)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (5, 5, 6, NULL, 0, 0)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (6, 2, 20, N'i want for guide my trip', 0, 0)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (7, 5, 20, N'i want for guide my trip', 0, 0)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (8, 7, 20, N'i want for guide my trip', 0, 0)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (9, 11, 20, N'i want for guide my trip', 0, 0)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (10, 12, 20, N'i want for guide my trip', 1, 4)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (11, 13, 20, N'i want for guide my trip', 0, 0)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (12, 14, 20, N'i want for guide my trip', 0, 0)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (13, 15, 20, N'i want for guide my trip', 0, 0)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (14, 16, 20, N'i want for guide my trip', 0, 0)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (15, 17, 20, N'i want for guide my trip', 0, 0)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (16, 18, 20, N'i want for guide my trip', 0, 0)
GO
INSERT [dbo].[tblHostRequests] ([RequestId], [HostId], [GuestId], [Message], [IsAccepted], [RatingValue]) VALUES (17, 19, 20, N'i want for guide my trip', 0, 0)
GO
SET IDENTITY_INSERT [dbo].[tblHostRequests] OFF
GO
SET IDENTITY_INSERT [dbo].[tblUser] ON 
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (1, N'boye ecec', N'boyecec565@laraskey.com', N'test', N'03123456789', N'Gujrat', N'Service Mor', 1, 0, 1, 1)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (2, N'test', N'test', N'test', N'211919191991', N'Lahore', N'Gawal Mandi', 2, 0, 1, 1)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (3, N'admin', N'admin', N'admin', N'1234567897', N'Gujrat', N'Service Mor', 3, 0, 1, 1)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (4, N'resihem', N'resihem219@iludir.com', N'test', N'0123456789', N'Gujrat', N'Service Mor', 1, 0, 1, 1)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (5, N'abrar siddique', N'abrarch2530@gmail.com', N'abrar2530', N'03454274426', N'Gujrat', N'kakrali', 2, 1, 1, 0)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (6, N'Bilal Afzaal', N'abrarsiddique928@gmail.com', N'abrar928', N'03034566859', N'gujrat', N'Pakistan', 1, 0, 1, 1)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (7, N'Ali ', N'host426426@gmail.com', N'Helloworld', N'03454274426', N'Gujrat', N'Pakistan', 2, 1, 1, 1)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (10, N'Rehman Ch', N'abdysaifali@gmail.com', N'nayyar123', N'03014275725', N'kharain', N'Pakistan', 1, 0, 1, 1)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (11, N'Ahman', N'nayyar50iqbal@gmail.com', N'nayyar50', N'03034566859', N'jhelum', N'pakistan', 2, 0, 1, 1)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (12, N'Muhammad Abrar', N'abrarsiddique2530@gmail.com', NULL, NULL, N'Bhimber', N'Azad kashmir', 2, 0, 1, 1)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (13, N'farhan', N'abrarsiddique2530@gmail.com', N'abrar123', N'03457442602', N'Jhelum', N'Pakistan', 2, 0, 0, 0)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (14, N'farhan', N'abrarsiddique2530@gmail.com', N'abrar123', N'03457442602', N'Jhelum', N'Pakistan', 2, 0, 0, 0)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (15, N'farhan', N'abrarsiddique2530@gmail.com', N'abrar123', N'03457442602', N'Jhelum', N'Pakistan', 2, 0, 0, 0)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (16, N'farhan', N'abrarsiddique2530@gmail.com', N'abrar123', N'03457442602', N'Jhelum', N'Pakistan', 2, 0, 0, 0)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (17, N'farhan', N'abrarsiddique2530@gmail.com', N'abrar123', N'03457442602', N'Jhelum', N'Pakistan', 2, 0, 0, 0)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (18, N'farhan', N'abrarsiddique2530@gmail.com', N'abrar123', N'03457442602', N'Jhelum', N'Pakistan', 2, 0, 1, 1)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (19, N'farhan', N'17074198-037@uog.edu.pk', N'farhan123', N'03456324170', N'Bhimber', N'Azad kashmir', 2, 0, 1, 1)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (20, N'laiba', N'nepag48483@troikos.com', N'laiba12', N'03427425452', N'Kharain', N'Pakistan', 1, 1, 1, 1)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (21, N'Muhammad ALi', N'touthost1@gmail.com', N'Abrar@12', N'03454274426', N'Gujrat', N'Pakistan', 1, 0, 0, 0)
GO
INSERT [dbo].[tblUser] ([UserId], [Full_Name], [Email], [Password], [Contact_Number], [Location_City], [Location_Area], [User_Type], [Fake_Reported_Count], [Is_Verified], [Is_Enabled]) VALUES (22, N'Nayyar Bamta', N'tourguest1@gmail.com', N'Nayyar@1', N'03427442508', N'Kharain', N'Pakistan', 2, 0, 1, 1)
GO
SET IDENTITY_INSERT [dbo].[tblUser] OFF
GO
SET IDENTITY_INSERT [dbo].[tblUserType] ON 
GO
INSERT [dbo].[tblUserType] ([UserTypeId], [UserTypeName]) VALUES (1, N'Guest')
GO
INSERT [dbo].[tblUserType] ([UserTypeId], [UserTypeName]) VALUES (2, N'Host')
GO
INSERT [dbo].[tblUserType] ([UserTypeId], [UserTypeName]) VALUES (3, N'Admin')
GO
SET IDENTITY_INSERT [dbo].[tblUserType] OFF
GO
ALTER TABLE [dbo].[tblGuestRequests] ADD  CONSTRAINT [DF_tblGuestRequests_RatingValue]  DEFAULT ((0)) FOR [RatingValue]
GO
ALTER TABLE [dbo].[tblHostRequests] ADD  CONSTRAINT [DF_tblHostRequests_RatingValue_1]  DEFAULT ((0)) FOR [RatingValue]
GO
USE [master]
GO
ALTER DATABASE [TourEaseDb] SET  READ_WRITE 
GO
