Create table Studenci(
	indeks int NOT NULL PRIMARY KEY,
	imie varchar(255) NOT NULL,
	nazwisko varchar(255) NOT NULL,
	mail varchar(255)
	);

Create table Prowadzacy(
	id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	imie varchar(255) NOT NULL,
	nazwisko varchar(255) NOT NULL,
	mail varchar(255),
	adname varchar(255) NOT NULL
	);

Create table Przedmioty(
	id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	id_prowadzacego int foreign key REFERENCES Prowadzacy(id),
	nazwa varchar(255)
);
Create table Sale(
	id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	nazwa varchar(255) NOT NULL
);
Create table Aplikacje(
	id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	id_sali int foreign key REFERENCES Sale(id),
	nazwa_komputera varchar(255)
);
Create table Grupy(
	id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	nazwa varchar(255) NOT NULL
	);
Create table Zajecia(
	id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	id_grupy int foreign key REFERENCES Grupy(id),
	rozpoczecie DATETIME NOT NULL,
	zakonczenie DATETIME NOT NULL,
	id_przedmiotu int foreign key REFERENCES Przedmioty(id),
	id_sali int foreign key REFERENCES Sale(id)
);
Create table StudenciGrupy(
	id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	id_studenta int foreign key REFERENCES Studenci(indeks),
	id_grupy int foreign key REFERENCES Grupy(id)
);

Create table Obecnosci(
	id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	id_studenta int foreign key REFERENCES Studenci(indeks),
	id_zajec int foreign key REFERENCES Zajecia(id),
	obecnosc bit DEFAULT 0,
	usprawiedliwienie bit DEFAULT 0
); 

INSERT INTO Studenci (indeks, imie, nazwisko, mail)
VALUES (111111, 'Jan', 'Kowalski', 'jakismail@domena');

INSERT INTO Prowadzacy (imie, nazwisko, mail, adname)
VALUES ('Konrad', 'Admin', 'szachkonrad@op.pl', 'LAPTOP-S1S79EF2\Konrad');

INSERT INTO Studenci (indeks, imie, nazwisko, mail)
VALUES (222222, 'Andrzej', 'Nowak', 'xyz@mail.com');

INSERT INTO Prowadzacy (imie, nazwisko, mail, adname)
VALUES ('Konrad2', 'Admin2', 'szachkonrad@op.pl', 'xyz');

INSERT INTO Przedmioty (id_prowadzacego, nazwa)
VALUES (1, 'PP');

INSERT INTO Przedmioty (id_prowadzacego, nazwa)
VALUES (2, 'UP');

INSERT INTO Sale (nazwa)
VALUES ('242');

INSERT INTO Aplikacje (id_sali, nazwa_komputera)
VALUES (1, 'R1');

INSERT INTO Grupy (nazwa)
VALUES ('INF3L1');

INSERT INTO Zajecia (id_grupy, rozpoczecie, zakonczenie, id_sali, id_przedmiotu)
VALUES (1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 1, 1);

INSERT INTO StudenciGrupy (id_studenta, id_grupy)
VALUES (111111,1);

INSERT INTO StudenciGrupy (id_studenta, id_grupy)
VALUES (222222,1);

INSERT INTO Obecnosci (id_studenta, id_zajec)
VALUES (111111, 1);

INSERT INTO Obecnosci (id_studenta, id_zajec)
VALUES (222222, 1);