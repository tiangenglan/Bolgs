



create database Bolgs


create table Tbbolgs
(
   ID int identity(1,1),
   bolgname varchar(50),
   author   varchar(20),
   times   datetime default getdate(),
   article  text,
   picture varchar(50),
   details varchar(50),
   states  varchar(50)
)

create table TbUser
(
   ID int identity(1,1),
   username varchar(50),
   password varchar(20),
   role varchar(20),
   loginip varchar(15),
   states varchar(50)
)

insert into Tbbolgs(bolgname,author,times,article,picture,details,states) values('�ҵĲ���','�����','','�ҵĲ����ҵĲ���','/..../','����','��ע')
insert into Tbbolgs(bolgname,author,article,picture,details,states) values('�ҵĲ���','�����','�ҵĲ����ҵĲ���','/..../','����','��ע')
insert into Tbbolgs(bolgname,author,article,picture,details,states) values('�ҵĲ���','�����','�ҵĲ����ҵĲ���','/..../','����','��ע')
insert into Tbbolgs(bolgname,author,article,picture,details,states) values('�ҵĲ���','�����','�ҵĲ����ҵĲ���','/..../','����','��ע')


select * from Tbbolgs

delete from  Tbbolgs