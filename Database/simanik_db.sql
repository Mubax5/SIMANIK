-- MySQL dump 10.13  Distrib 8.0.30, for Win64 (x86_64)
--
-- Host: localhost    Database: simanik_db
-- ------------------------------------------------------
-- Server version	8.0.30

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `simanik_db`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `simanik_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `simanik_db`;

--
-- Table structure for table `diseases`
--

DROP TABLE IF EXISTS `diseases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `diseases` (
  `DiseaseId` int NOT NULL AUTO_INCREMENT,
  `DiseaseCode` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `DiseaseName` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`DiseaseId`),
  UNIQUE KEY `DiseaseCode` (`DiseaseCode`)
) ENGINE=InnoDB AUTO_INCREMENT=200 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `diseases`
--

LOCK TABLES `diseases` WRITE;
/*!40000 ALTER TABLE `diseases` DISABLE KEYS */;
INSERT INTO `diseases` VALUES (1,'FLU','Influenza','Infeksi saluran pernapasan akibat virus',1),(2,'DEM','Demam','Peningkatan suhu tubuh',1),(3,'BAT','Batuk','Gejala gangguan saluran pernapasan',1),(4,'HIP','Hipertensi','Tekanan darah tinggi',1),(5,'DIA','Diabetes Mellitus','Gangguan kadar gula darah',1),(100,'ISPA','ISPA','Infeksi saluran pernapasan akut',1),(101,'MAAG','Gastritis','Peradangan lambung atau nyeri maag',1),(102,'ALR','Alergi','Reaksi alergi ringan sampai sedang',1),(103,'DER','Dermatitis','Peradangan pada kulit',1),(104,'MIG','Migrain','Sakit kepala sebelah berulang',1),(105,'THT','Radang Tenggorokan','Peradangan pada tenggorokan',1),(106,'DIARE','Diare','Frekuensi BAB meningkat dengan konsistensi cair',1),(107,'KOL','Kolesterol Tinggi','Kadar kolesterol melebihi normal',1),(108,'ASMA','Asma','Gangguan pernapasan berulang',1),(109,'GIGI','Karies Gigi','Kerusakan gigi akibat plak dan bakteri',1);
/*!40000 ALTER TABLE `diseases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `doctor_schedules`
--

DROP TABLE IF EXISTS `doctor_schedules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doctor_schedules` (
  `ScheduleId` int NOT NULL AUTO_INCREMENT,
  `DoctorId` int NOT NULL,
  `ScheduleDate` date NOT NULL,
  `StartTime` time NOT NULL,
  `EndTime` time NOT NULL,
  `Quota` int NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`ScheduleId`),
  KEY `DoctorId` (`DoctorId`),
  CONSTRAINT `doctor_schedules_ibfk_1` FOREIGN KEY (`DoctorId`) REFERENCES `doctors` (`DoctorId`)
) ENGINE=InnoDB AUTO_INCREMENT=201 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `doctor_schedules`
--

LOCK TABLES `doctor_schedules` WRITE;
/*!40000 ALTER TABLE `doctor_schedules` DISABLE KEYS */;
INSERT INTO `doctor_schedules` VALUES (100,100,'2026-04-21','08:00:00','11:00:00',12,1),(101,101,'2026-04-21','13:00:00','15:00:00',10,1),(102,102,'2026-04-21','16:00:00','18:00:00',8,1),(103,101,'2026-04-22','08:00:00','11:00:00',12,1),(104,102,'2026-04-22','13:00:00','15:00:00',10,1),(105,103,'2026-04-22','16:00:00','18:00:00',8,1),(106,102,'2026-04-23','08:00:00','11:00:00',12,1),(107,103,'2026-04-23','13:00:00','15:00:00',10,1),(108,104,'2026-04-23','16:00:00','18:00:00',8,1),(109,103,'2026-04-24','08:00:00','11:00:00',12,1),(110,104,'2026-04-24','13:00:00','15:00:00',10,1),(111,105,'2026-04-24','16:00:00','18:00:00',8,1),(112,104,'2026-04-25','08:00:00','11:00:00',12,1),(113,105,'2026-04-25','13:00:00','15:00:00',10,1),(114,100,'2026-04-25','16:00:00','18:00:00',8,1),(115,105,'2026-04-26','08:00:00','11:00:00',12,1),(116,100,'2026-04-26','13:00:00','15:00:00',10,1),(117,101,'2026-04-26','16:00:00','18:00:00',8,1),(118,100,'2026-04-27','08:00:00','11:00:00',12,0),(119,101,'2026-04-27','13:00:00','15:00:00',10,1),(120,102,'2026-04-27','16:00:00','18:00:00',8,1),(121,101,'2026-04-28','08:00:00','11:00:00',12,1),(122,102,'2026-04-28','13:00:00','15:00:00',10,1),(123,103,'2026-04-28','16:00:00','18:00:00',8,1),(124,102,'2026-04-29','08:00:00','11:00:00',12,1),(125,103,'2026-04-29','13:00:00','15:00:00',10,1),(126,104,'2026-04-29','16:00:00','18:00:00',8,1),(127,103,'2026-04-30','08:00:00','11:00:00',12,1),(128,104,'2026-04-30','13:00:00','15:00:00',10,1),(129,105,'2026-04-30','16:00:00','18:00:00',8,1),(130,104,'2026-05-01','08:00:00','11:00:00',12,1),(131,105,'2026-05-01','13:00:00','15:00:00',10,1),(132,100,'2026-05-01','16:00:00','18:00:00',8,1),(133,105,'2026-05-02','08:00:00','11:00:00',12,1),(134,100,'2026-05-02','13:00:00','15:00:00',10,1),(135,101,'2026-05-02','16:00:00','18:00:00',8,1),(136,100,'2026-05-03','08:00:00','11:00:00',12,1),(137,101,'2026-05-03','13:00:00','15:00:00',10,1),(138,102,'2026-05-03','16:00:00','18:00:00',8,1),(139,101,'2026-05-04','08:00:00','11:00:00',12,1),(140,102,'2026-05-04','13:00:00','15:00:00',10,1),(141,103,'2026-05-04','16:00:00','18:00:00',8,1),(200,100,'2026-04-27','08:00:00','11:00:00',12,1);
/*!40000 ALTER TABLE `doctor_schedules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `doctors`
--

DROP TABLE IF EXISTS `doctors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doctors` (
  `DoctorId` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `FullName` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Specialization` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `PhoneNumber` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`DoctorId`),
  UNIQUE KEY `UserId` (`UserId`),
  CONSTRAINT `doctors_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=200 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `doctors`
--

LOCK TABLES `doctors` WRITE;
/*!40000 ALTER TABLE `doctors` DISABLE KEYS */;
INSERT INTO `doctors` VALUES (100,100,'dr. Budi Santoso','Dokter Umum','081211110001',1),(101,101,'drg. Siti Rahma','Dokter Gigi','081211110002',1),(102,102,'dr. Andi Wijaya, Sp.A','Dokter Anak','081211110003',1),(103,103,'dr. Maya Lestari, Sp.PD','Penyakit Dalam','081211110004',1),(104,104,'dr. Rizky Pratama, Sp.THT','THT','081211110005',1),(105,105,'dr. Nur Aini, Sp.KK','Kulit dan Kelamin','081211110006',1);
/*!40000 ALTER TABLE `doctors` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `examinations`
--

DROP TABLE IF EXISTS `examinations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `examinations` (
  `ExaminationId` int NOT NULL AUTO_INCREMENT,
  `VisitId` int NOT NULL,
  `DoctorId` int NOT NULL,
  `DiseaseId` int NOT NULL,
  `ExaminationDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CurrentComplaint` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `DiagnosisNotes` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TreatmentNotes` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`ExaminationId`),
  UNIQUE KEY `VisitId` (`VisitId`),
  KEY `DoctorId` (`DoctorId`),
  KEY `DiseaseId` (`DiseaseId`),
  CONSTRAINT `examinations_ibfk_1` FOREIGN KEY (`VisitId`) REFERENCES `visits` (`VisitId`),
  CONSTRAINT `examinations_ibfk_2` FOREIGN KEY (`DoctorId`) REFERENCES `doctors` (`DoctorId`),
  CONSTRAINT `examinations_ibfk_3` FOREIGN KEY (`DiseaseId`) REFERENCES `diseases` (`DiseaseId`)
) ENGINE=InnoDB AUTO_INCREMENT=200 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `examinations`
--

LOCK TABLES `examinations` WRITE;
/*!40000 ALTER TABLE `examinations` DISABLE KEYS */;
INSERT INTO `examinations` VALUES (100,100,100,105,'2026-04-21 09:25:00','Demam dan sakit kepala sejak kemarin','Diagnosa sesuai gejala: demam dan sakit kepala sejak kemarin','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(101,102,102,106,'2026-04-22 08:45:00','Kulit merah dan gatal','Diagnosa sesuai gejala: kulit merah dan gatal','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(102,103,103,107,'2026-04-23 09:15:00','Pusing berulang','Diagnosa sesuai gejala: pusing berulang','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(103,104,104,108,'2026-04-23 09:25:00','Kontrol gula darah','Diagnosa sesuai gejala: kontrol gula darah','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(104,106,104,109,'2026-04-25 08:45:00','Demam dan sakit kepala sejak kemarin','Diagnosa sesuai gejala: demam dan sakit kepala sejak kemarin','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(105,107,105,1,'2026-04-26 09:15:00','Kontrol tekanan darah','Diagnosa sesuai gejala: kontrol tekanan darah','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(106,108,100,2,'2026-04-26 09:25:00','Kulit merah dan gatal','Diagnosa sesuai gejala: kulit merah dan gatal','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(107,110,102,3,'2026-04-27 08:45:00','Kontrol gula darah','Diagnosa sesuai gejala: kontrol gula darah','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(108,111,103,4,'2026-04-28 09:15:00','Nyeri sendi dan badan pegal','Diagnosa sesuai gejala: nyeri sendi dan badan pegal','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(109,112,102,5,'2026-04-29 09:25:00','Demam dan sakit kepala sejak kemarin','Diagnosa sesuai gejala: demam dan sakit kepala sejak kemarin','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(110,114,104,100,'2026-04-30 08:45:00','Kulit merah dan gatal','Diagnosa sesuai gejala: kulit merah dan gatal','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(111,115,105,101,'2026-05-01 09:15:00','Pusing berulang','Diagnosa sesuai gejala: pusing berulang','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(112,116,100,102,'2026-05-01 09:25:00','Kontrol gula darah','Diagnosa sesuai gejala: kontrol gula darah','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(113,118,100,103,'2026-05-03 08:45:00','Demam dan sakit kepala sejak kemarin','Diagnosa sesuai gejala: demam dan sakit kepala sejak kemarin','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(114,119,101,104,'2026-05-04 09:15:00','Kontrol tekanan darah','Diagnosa sesuai gejala: kontrol tekanan darah','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(115,120,102,105,'2026-05-04 09:25:00','Kulit merah dan gatal','Diagnosa sesuai gejala: kulit merah dan gatal','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(116,122,102,106,'2026-04-21 08:45:00','Kontrol gula darah','Diagnosa sesuai gejala: kontrol gula darah','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.'),(117,123,103,107,'2026-04-22 09:15:00','Nyeri sendi dan badan pegal','Diagnosa sesuai gejala: nyeri sendi dan badan pegal','Istirahat cukup, konsumsi obat sesuai aturan, kontrol ulang bila keluhan berlanjut.');
/*!40000 ALTER TABLE `examinations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medical_records`
--

DROP TABLE IF EXISTS `medical_records`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medical_records` (
  `RecordId` int NOT NULL AUTO_INCREMENT,
  `PatientId` int NOT NULL,
  `LastVisitDate` datetime DEFAULT NULL,
  `BloodType` varchar(5) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `AllergyNotes` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ChronicDiseaseNotes` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`RecordId`),
  UNIQUE KEY `PatientId` (`PatientId`),
  CONSTRAINT `medical_records_ibfk_1` FOREIGN KEY (`PatientId`) REFERENCES `patients` (`PatientId`)
) ENGINE=InnoDB AUTO_INCREMENT=201 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medical_records`
--

LOCK TABLES `medical_records` WRITE;
/*!40000 ALTER TABLE `medical_records` DISABLE KEYS */;
INSERT INTO `medical_records` VALUES (2,2,'2026-04-21 09:25:00','O','Tidak ada','Tidak ada'),(100,100,NULL,'A','Debu','Tidak ada'),(101,101,'2026-05-03 08:45:00','B','Amoxicillin','Asma ringan'),(102,102,'2026-04-27 08:45:00','AB','Tidak ada','Hipertensi'),(103,103,'2026-04-22 08:45:00','O','Seafood','Tidak ada'),(104,104,'2026-05-04 09:15:00','A','Tidak ada','Tidak ada'),(105,105,'2026-05-04 09:25:00','B','Tidak ada','Diabetes Mellitus'),(106,106,'2026-04-29 09:25:00','O','Udang','Tidak ada'),(107,107,'2026-04-23 09:25:00','AB','Tidak ada','Hipertensi'),(108,108,NULL,'A','Tidak ada','Tidak ada'),(109,109,'2026-04-21 08:45:00','O','Tidak ada','Asam lambung'),(110,110,'2026-04-30 08:45:00','B','Aspirin','Tidak ada'),(111,111,'2026-04-25 08:45:00','O','Tidak ada','Kolesterol'),(112,112,'2026-04-22 09:15:00','A','Tidak ada','Tidak ada'),(113,113,'2026-05-01 09:15:00','B','Tidak ada','Tidak ada'),(114,114,'2026-05-01 09:25:00','AB','Tidak ada','Hipertensi'),(115,115,'2026-04-26 09:25:00','O','Tidak ada','Tidak ada'),(200,200,'2026-04-26 23:47:58',NULL,'','');
/*!40000 ALTER TABLE `medical_records` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicines`
--

DROP TABLE IF EXISTS `medicines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicines` (
  `MedicineId` int NOT NULL AUTO_INCREMENT,
  `MedicineName` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `MedicineType` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Stock` int NOT NULL DEFAULT '0',
  `Unit` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DefaultInstruction` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`MedicineId`)
) ENGINE=InnoDB AUTO_INCREMENT=201 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicines`
--

LOCK TABLES `medicines` WRITE;
/*!40000 ALTER TABLE `medicines` DISABLE KEYS */;
INSERT INTO `medicines` VALUES (1,'Paracetamol','Tablet',100,'tablet','3x1 setelah makan',1),(2,'Amoxicillin','Kapsul',50,'kapsul','3x1 setelah makan',1),(3,'OBH','Sirup',30,'botol','3x1 sendok makan',1),(100,'Cetirizine','Tablet',80,'tablet','1x1 malam hari',1),(101,'Ibuprofen','Tablet',75,'tablet','2x1 setelah makan bila nyeri',1),(102,'Antasida DOEN','Tablet Kunyah',90,'tablet','3x1 sebelum makan',1),(103,'Vitamin C','Tablet',120,'tablet','1x1 setelah makan',1),(104,'Loratadine','Tablet',65,'tablet','1x1 sehari',1),(105,'Salbutamol','Tablet',45,'tablet','3x1 sesuai anjuran dokter',1),(106,'Metformin','Tablet',60,'tablet','2x1 setelah makan',1),(107,'Amlodipine','Tablet',55,'tablet','1x1 malam hari',1),(108,'Simvastatin','Tablet',40,'tablet','1x1 malam hari',1),(109,'Oralit','Sachet',150,'sachet','Larutkan 1 sachet dalam 200 ml air',1),(110,'Betadine','Cairan Antiseptik',25,'botol','Oleskan pada luka setelah dibersihkan',1),(111,'Hydrocortisone','Salep',35,'tube','Oles tipis 2x sehari',1),(112,'Ambroxol','Tablet',70,'tablet','3x1 setelah makan',0),(113,'Cefixime','Kapsul',28,'kapsul','2x1 setelah makan',1),(114,'Chlorpheniramine','Tablet',95,'tablet','3x1 setelah makan',1),(115,'Omeprazole','Kapsul',52,'kapsul','1x1 sebelum makan pagi',1),(116,'Minyak Kayu Putih','Cairan',10,'botol','Gunakan seperlunya',1),(117,'Zinc','Tablet',2,'tablet','1x1 setelah makan',1),(200,'Ambroxol','Tablet',70,'tablet','3x1 setelah makan',1);
/*!40000 ALTER TABLE `medicines` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patients`
--

DROP TABLE IF EXISTS `patients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patients` (
  `PatientId` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `PatientNumber` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `FullName` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `BirthDate` date NOT NULL,
  `Gender` enum('Laki-laki','Perempuan') COLLATE utf8mb4_unicode_ci NOT NULL,
  `Address` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PhoneNumber` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`PatientId`),
  UNIQUE KEY `UserId` (`UserId`),
  UNIQUE KEY `PatientNumber` (`PatientNumber`),
  CONSTRAINT `patients_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=201 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patients`
--

LOCK TABLES `patients` WRITE;
/*!40000 ALTER TABLE `patients` DISABLE KEYS */;
INSERT INTO `patients` VALUES (2,3,'RM000001','Hilmi Mubarok','2006-04-26','Laki-laki','Jl. Manunggal','085711228699'),(100,200,'RM000100','Andi Saputra','1998-01-12','Laki-laki','Jl. Melati No. 10','081300000100'),(101,201,'RM000101','Sari Wulandari','2001-05-20','Perempuan','Jl. Kenanga No. 21','081300000101'),(102,202,'RM000102','Dewi Anggraini','1995-09-03','Perempuan','Jl. Mawar No. 5','081300000102'),(103,203,'RM000103','Rizal Maulana','1992-11-15','Laki-laki','Jl. Flamboyan No. 8','081300000103'),(104,204,'RM000104','Nadia Putri','2004-07-30','Perempuan','Jl. Cempaka No. 12','081300000104'),(105,205,'RM000105','Fajar Hidayat','1988-02-11','Laki-laki','Jl. Anggrek No. 14','081300000105'),(106,206,'RM000106','Lina Marlina','1999-12-01','Perempuan','Jl. Dahlia No. 17','081300000106'),(107,207,'RM000107','Bayu Prakoso','1985-06-18','Laki-laki','Jl. Teratai No. 3','081300000107'),(108,208,'RM000108','Rani Oktaviani','1997-10-23','Perempuan','Jl. Sakura No. 4','081300000108'),(109,209,'RM000109','Dimas Ardiansyah','1990-03-09','Laki-laki','Jl. Kamboja No. 9','081300000109'),(110,210,'RM000110','Ayu Permatasari','2002-08-14','Perempuan','Jl. Bougenville No. 7','081300000110'),(111,211,'RM000111','Yusuf Ramadhan','1978-04-02','Laki-laki','Jl. Merpati No. 1','081300000111'),(112,212,'RM000112','Intan Prameswari','1996-01-28','Perempuan','Jl. Nusa Indah No. 19','081300000112'),(113,213,'RM000113','Kevin Alfarizi','2012-11-22','Laki-laki','Jl. Garuda No. 2','081300000113'),(114,214,'RM000114','Maya Salsabila','1982-09-17','Perempuan','Jl. Rajawali No. 6','081300000114'),(115,215,'RM000115','Eko Prasetyo','1993-05-06','Laki-laki','Jl. Elang No. 11','081300000115'),(200,300,'RM000116','fairuz','2006-04-26','Laki-laki','Jl Jl','08123456678910');
/*!40000 ALTER TABLE `patients` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prescription_details`
--

DROP TABLE IF EXISTS `prescription_details`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prescription_details` (
  `PrescriptionDetailId` int NOT NULL AUTO_INCREMENT,
  `ExaminationId` int NOT NULL,
  `MedicineId` int NOT NULL,
  `Quantity` int NOT NULL,
  `InstructionNote` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`PrescriptionDetailId`),
  KEY `ExaminationId` (`ExaminationId`),
  KEY `MedicineId` (`MedicineId`),
  CONSTRAINT `prescription_details_ibfk_1` FOREIGN KEY (`ExaminationId`) REFERENCES `examinations` (`ExaminationId`),
  CONSTRAINT `prescription_details_ibfk_2` FOREIGN KEY (`MedicineId`) REFERENCES `medicines` (`MedicineId`)
) ENGINE=InnoDB AUTO_INCREMENT=300 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prescription_details`
--

LOCK TABLES `prescription_details` WRITE;
/*!40000 ALTER TABLE `prescription_details` DISABLE KEYS */;
INSERT INTO `prescription_details` VALUES (100,100,1,5,'3x1 setelah makan'),(101,100,3,8,'2x1 setelah makan'),(102,101,3,8,'2x1 setelah makan'),(103,101,100,10,'1x1 malam hari'),(104,102,100,10,'1x1 malam hari'),(105,102,101,12,'Bila perlu saat nyeri'),(106,102,102,15,'Larutkan sesuai aturan'),(107,103,101,12,'Bila perlu saat nyeri'),(108,103,102,15,'Larutkan sesuai aturan'),(109,104,102,15,'Larutkan sesuai aturan'),(110,104,103,5,'3x1 setelah makan'),(111,105,103,5,'3x1 setelah makan'),(112,105,107,8,'2x1 setelah makan'),(113,105,109,10,'1x1 malam hari'),(114,106,107,8,'2x1 setelah makan'),(115,106,109,10,'1x1 malam hari'),(116,107,109,10,'1x1 malam hari'),(117,107,112,12,'Bila perlu saat nyeri'),(118,108,112,12,'Bila perlu saat nyeri'),(119,108,115,15,'Larutkan sesuai aturan'),(120,108,1,5,'3x1 setelah makan'),(121,109,115,15,'Larutkan sesuai aturan'),(122,109,1,5,'3x1 setelah makan'),(123,110,1,5,'3x1 setelah makan'),(124,110,3,8,'2x1 setelah makan'),(125,111,3,8,'2x1 setelah makan'),(126,111,100,10,'1x1 malam hari'),(127,111,101,12,'Bila perlu saat nyeri'),(128,112,100,10,'1x1 malam hari'),(129,112,101,12,'Bila perlu saat nyeri'),(130,113,101,12,'Bila perlu saat nyeri'),(131,113,102,15,'Larutkan sesuai aturan'),(132,114,102,15,'Larutkan sesuai aturan'),(133,114,103,5,'3x1 setelah makan'),(134,114,107,8,'2x1 setelah makan'),(135,115,103,5,'3x1 setelah makan'),(136,115,107,8,'2x1 setelah makan'),(137,116,107,8,'2x1 setelah makan'),(138,116,109,10,'1x1 malam hari'),(139,117,109,10,'1x1 malam hari'),(140,117,112,12,'Bila perlu saat nyeri'),(141,117,115,15,'Larutkan sesuai aturan');
/*!40000 ALTER TABLE `prescription_details` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reservations`
--

DROP TABLE IF EXISTS `reservations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reservations` (
  `ReservationId` int NOT NULL AUTO_INCREMENT,
  `PatientId` int NOT NULL,
  `ScheduleId` int NOT NULL,
  `Complaint` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ReservationStatus` enum('Menunggu Verifikasi','Dikonfirmasi','Ditolak','Dibatalkan Pasien','Check-in','Selesai') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Menunggu Verifikasi',
  `RejectionReason` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`ReservationId`),
  KEY `PatientId` (`PatientId`),
  KEY `ScheduleId` (`ScheduleId`),
  CONSTRAINT `reservations_ibfk_1` FOREIGN KEY (`PatientId`) REFERENCES `patients` (`PatientId`),
  CONSTRAINT `reservations_ibfk_2` FOREIGN KEY (`ScheduleId`) REFERENCES `doctor_schedules` (`ScheduleId`)
) ENGINE=InnoDB AUTO_INCREMENT=201 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reservations`
--

LOCK TABLES `reservations` WRITE;
/*!40000 ALTER TABLE `reservations` DISABLE KEYS */;
INSERT INTO `reservations` VALUES (100,2,100,'Demam dan sakit kepala sejak kemarin','Selesai',NULL,'2026-04-20 08:00:00'),(101,100,101,'Batuk pilek dan tenggorokan gatal','Menunggu Verifikasi',NULL,'2026-04-21 09:07:00'),(102,101,102,'Nyeri gigi saat makan manis','Dikonfirmasi',NULL,'2026-04-22 10:14:00'),(103,102,103,'Kontrol tekanan darah','Check-in',NULL,'2026-04-23 11:21:00'),(104,103,104,'Kulit merah dan gatal','Selesai',NULL,'2026-04-24 12:28:00'),(105,104,105,'Mual dan nyeri ulu hati','Ditolak','Jadwal dokter penuh, silakan pilih jadwal lain','2026-04-25 13:35:00'),(106,105,106,'Sesak ringan saat malam hari','Dibatalkan Pasien',NULL,'2026-04-26 14:42:00'),(107,106,107,'Pusing berulang','Selesai',NULL,'2026-04-27 15:49:00'),(108,107,108,'Kontrol gula darah','Selesai',NULL,'2026-04-28 08:56:00'),(109,108,109,'Diare sejak pagi','Menunggu Verifikasi',NULL,'2026-04-29 09:03:00'),(110,109,110,'Telinga terasa nyeri','Dikonfirmasi',NULL,'2026-04-30 10:10:00'),(111,110,111,'Nyeri sendi dan badan pegal','Check-in',NULL,'2026-05-01 11:17:00'),(112,111,112,'Demam dan sakit kepala sejak kemarin','Selesai',NULL,'2026-04-20 12:24:00'),(113,112,113,'Batuk pilek dan tenggorokan gatal','Ditolak','Jadwal dokter penuh, silakan pilih jadwal lain','2026-04-21 13:31:00'),(114,113,114,'Nyeri gigi saat makan manis','Dibatalkan Pasien',NULL,'2026-04-22 14:38:00'),(115,114,115,'Kontrol tekanan darah','Selesai',NULL,'2026-04-23 15:45:00'),(116,115,116,'Kulit merah dan gatal','Selesai',NULL,'2026-04-24 08:52:00'),(117,2,117,'Mual dan nyeri ulu hati','Menunggu Verifikasi',NULL,'2026-04-25 09:59:00'),(118,100,118,'Sesak ringan saat malam hari','Dikonfirmasi',NULL,'2026-04-26 10:06:00'),(119,101,119,'Pusing berulang','Check-in',NULL,'2026-04-27 11:13:00'),(120,102,120,'Kontrol gula darah','Selesai',NULL,'2026-04-28 12:20:00'),(121,103,121,'Diare sejak pagi','Ditolak','Jadwal dokter penuh, silakan pilih jadwal lain','2026-04-29 13:27:00'),(122,104,122,'Telinga terasa nyeri','Dibatalkan Pasien',NULL,'2026-04-30 14:34:00'),(123,105,123,'Nyeri sendi dan badan pegal','Selesai',NULL,'2026-05-01 15:41:00'),(124,106,124,'Demam dan sakit kepala sejak kemarin','Selesai',NULL,'2026-04-20 08:48:00'),(125,107,125,'Batuk pilek dan tenggorokan gatal','Menunggu Verifikasi',NULL,'2026-04-21 09:55:00'),(126,108,126,'Nyeri gigi saat makan manis','Dikonfirmasi',NULL,'2026-04-22 10:02:00'),(127,109,127,'Kontrol tekanan darah','Check-in',NULL,'2026-04-23 11:09:00'),(128,110,128,'Kulit merah dan gatal','Selesai',NULL,'2026-04-24 12:16:00'),(129,111,129,'Mual dan nyeri ulu hati','Ditolak','Jadwal dokter penuh, silakan pilih jadwal lain','2026-04-25 13:23:00'),(130,112,130,'Sesak ringan saat malam hari','Dibatalkan Pasien',NULL,'2026-04-26 14:30:00'),(131,113,131,'Pusing berulang','Selesai',NULL,'2026-04-27 15:37:00'),(132,114,132,'Kontrol gula darah','Selesai',NULL,'2026-04-28 08:44:00'),(133,115,133,'Diare sejak pagi','Menunggu Verifikasi',NULL,'2026-04-29 09:51:00'),(134,2,134,'Telinga terasa nyeri','Dikonfirmasi',NULL,'2026-04-30 10:58:00'),(135,100,135,'Nyeri sendi dan badan pegal','Check-in',NULL,'2026-05-01 11:05:00'),(136,101,136,'Demam dan sakit kepala sejak kemarin','Selesai',NULL,'2026-04-20 12:12:00'),(137,102,137,'Batuk pilek dan tenggorokan gatal','Ditolak','Jadwal dokter penuh, silakan pilih jadwal lain','2026-04-21 13:19:00'),(138,103,138,'Nyeri gigi saat makan manis','Dibatalkan Pasien',NULL,'2026-04-22 14:26:00'),(139,104,139,'Kontrol tekanan darah','Selesai',NULL,'2026-04-23 15:33:00'),(140,105,140,'Kulit merah dan gatal','Selesai',NULL,'2026-04-24 08:40:00'),(141,106,141,'Mual dan nyeri ulu hati','Menunggu Verifikasi',NULL,'2026-04-25 09:47:00'),(142,107,100,'Sesak ringan saat malam hari','Dikonfirmasi',NULL,'2026-04-26 10:54:00'),(143,108,101,'Pusing berulang','Check-in',NULL,'2026-04-27 11:01:00'),(144,109,102,'Kontrol gula darah','Selesai',NULL,'2026-04-28 12:08:00'),(145,110,103,'Diare sejak pagi','Ditolak','Jadwal dokter penuh, silakan pilih jadwal lain','2026-04-29 13:15:00'),(146,111,104,'Telinga terasa nyeri','Dibatalkan Pasien',NULL,'2026-04-30 14:22:00'),(147,112,105,'Nyeri sendi dan badan pegal','Selesai',NULL,'2026-05-01 15:29:00'),(200,100,127,'Dalam Hati','Dikonfirmasi',NULL,'2026-04-27 09:18:19');
/*!40000 ALTER TABLE `reservations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `UserId` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Password` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Role` enum('Admin','Dokter','Pasien') COLLATE utf8mb4_unicode_ci NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`UserId`),
  UNIQUE KEY `Username` (`Username`)
) ENGINE=InnoDB AUTO_INCREMENT=301 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin','240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9','Admin',1,'2026-04-26 17:43:41'),(3,'Muba','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3','Pasien',1,'2026-04-26 18:30:35'),(100,'dokter_umum','b3959dee9b178b030c2b8373da55a04ab3adb318edeb178953ff8b77301a360a','Dokter',1,'2026-04-26 20:00:00'),(101,'dokter_gigi','b3959dee9b178b030c2b8373da55a04ab3adb318edeb178953ff8b77301a360a','Dokter',1,'2026-04-26 20:00:00'),(102,'dokter_anak','b3959dee9b178b030c2b8373da55a04ab3adb318edeb178953ff8b77301a360a','Dokter',1,'2026-04-26 20:00:00'),(103,'dokter_penyakit_dalam','b3959dee9b178b030c2b8373da55a04ab3adb318edeb178953ff8b77301a360a','Dokter',1,'2026-04-26 20:00:00'),(104,'dokter_tht','b3959dee9b178b030c2b8373da55a04ab3adb318edeb178953ff8b77301a360a','Dokter',1,'2026-04-26 20:00:00'),(105,'dokter_kulit','b3959dee9b178b030c2b8373da55a04ab3adb318edeb178953ff8b77301a360a','Dokter',1,'2026-04-26 20:00:00'),(200,'pasien_andi','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(201,'pasien_sari','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(202,'pasien_dewi','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(203,'pasien_rizal','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(204,'pasien_nadia','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(205,'pasien_fajar','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(206,'pasien_lina','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(207,'pasien_bayu','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(208,'pasien_rani','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(209,'pasien_dimas','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(210,'pasien_ayu','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(211,'pasien_yusuf','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(212,'pasien_intan','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(213,'pasien_kevin','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(214,'pasien_maya','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(215,'pasien_eko','b3cb1bf1350e826eafc2250c570837e1ef1a0e8d6fece2c3478740670ce8fed1','Pasien',1,'2026-04-26 20:00:00'),(300,'pasien_lord','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3','Pasien',1,'2026-04-26 23:47:59');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `visits`
--

DROP TABLE IF EXISTS `visits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `visits` (
  `VisitId` int NOT NULL AUTO_INCREMENT,
  `ReservationId` int NOT NULL,
  `QueueNumber` int NOT NULL,
  `CheckInTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `VisitStatus` enum('Menunggu','Sedang Diperiksa','Selesai') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Menunggu',
  PRIMARY KEY (`VisitId`),
  UNIQUE KEY `ReservationId` (`ReservationId`),
  CONSTRAINT `visits_ibfk_1` FOREIGN KEY (`ReservationId`) REFERENCES `reservations` (`ReservationId`)
) ENGINE=InnoDB AUTO_INCREMENT=200 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `visits`
--

LOCK TABLES `visits` WRITE;
/*!40000 ALTER TABLE `visits` DISABLE KEYS */;
INSERT INTO `visits` VALUES (100,100,1,'2026-04-21 08:55:00','Selesai'),(101,103,1,'2026-04-22 09:25:00','Menunggu'),(102,104,2,'2026-04-22 08:15:00','Selesai'),(103,107,1,'2026-04-23 08:45:00','Selesai'),(104,108,2,'2026-04-23 08:55:00','Selesai'),(105,111,1,'2026-04-24 09:25:00','Menunggu'),(106,112,1,'2026-04-25 08:15:00','Selesai'),(107,115,1,'2026-04-26 08:45:00','Selesai'),(108,116,2,'2026-04-26 08:55:00','Selesai'),(109,119,1,'2026-04-27 09:25:00','Menunggu'),(110,120,2,'2026-04-27 08:15:00','Selesai'),(111,123,1,'2026-04-28 08:45:00','Selesai'),(112,124,1,'2026-04-29 08:55:00','Selesai'),(113,127,1,'2026-04-30 09:25:00','Menunggu'),(114,128,2,'2026-04-30 08:15:00','Selesai'),(115,131,1,'2026-05-01 08:45:00','Selesai'),(116,132,2,'2026-05-01 08:55:00','Selesai'),(117,135,1,'2026-05-02 09:25:00','Menunggu'),(118,136,1,'2026-05-03 08:15:00','Selesai'),(119,139,1,'2026-05-04 08:45:00','Selesai'),(120,140,2,'2026-05-04 08:55:00','Selesai'),(121,143,2,'2026-04-21 09:25:00','Menunggu'),(122,144,3,'2026-04-21 08:15:00','Selesai'),(123,147,3,'2026-04-22 08:45:00','Selesai');
/*!40000 ALTER TABLE `visits` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-04-27 10:52:41
