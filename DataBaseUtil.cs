using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace LoudSong
{
    // This class holds many string constants related to MySQL syntax in order to keep the program and its code cleaner.
    public class DataBaseUtil
    {
        public const string MySQLDatabaseConnection = "server=127.0.0.1; port=3306; Uid=root; pwd="; // DON'T FORGET TO CHANGE THE UID AND PASSWORD IN ORDER TO MAKE THE PROJECT WORK IN WHATEVER PC IT'S WORKING ON! TESTING WITH MY OWN'S (NOT WRITTEN FOR SECURITY).
        public const string MySQLCreateDatabase = "CREATE DATABASE IF NOT EXISTS PruebaLS;"; // For creating the Database.
        public const string MySQLUseDatabase = "USE PruebaLS;"; // For selecting the Database.
        public const string MySQLCreateTable = "CREATE TABLE IF NOT EXISTS song(" +
                                                    "title VARCHAR(50) PRIMARY KEY," +
                                                    "genre VARCHAR(50)," +
                                                    "lyrics VARCHAR(4000)," +
                                                    "duration VARCHAR(5)," +
                                                    "artist VARCHAR(50)," +
                                                    "album VARCHAR(50)," +
                                                    "year INT(4)," +
                                                    "isFavourite TINYINT(1)" +
                                                ");"; // For creating the main table.
    }
}
