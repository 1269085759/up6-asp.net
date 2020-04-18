using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using up6.db.biz;
using up6.db.biz.folder;
using up6.down2.biz;
using up6.filemgr.app;

namespace up6.db.database
{
    public class DBConfig
    {
        public bool isOracle = false;

        public DBConfig()
        {
            DbHelper db = new DbHelper();
            this.isOracle = db.isOracle();
        }

        public un_builder ub()
        {
            if (this.isOracle) return new un_builder_oracle();
            else return new un_builder();
        }

        public DBFile db()
        {
            if (this.isOracle) return new DBFileOracle();
            else return new DBFile();
        }

        public DBFolder folder()
        {
            if (this.isOracle) return new DBFolderOracle();
            else return new DBFolder();
        }

        public DnFile downF()
        {
            if (this.isOracle) return new DnFileOracle();
            else return new DnFile();
        }

        public DnFolder downFd()
        {
            if (this.isOracle) return new DnFolderOracle();
            else return new DnFolder();
        }

        public fd_scan sa()
        {
            if (this.isOracle) return new fd_scan_oracle();
            else return new fd_scan();
        }

        public SqlExec se()
        {
            if (this.isOracle) return new OracleExec();
            else return new SqlExec();
        }

        public DbBase bs()
        {
            if (this.isOracle) return new OracleDbBase();
            else return new DbBase();
        }

    }
}