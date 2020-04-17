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
        public string m_db = "sql";

        public bool isOracle()
        {
            return string.Compare(this.m_db, "oracle") == 0;
        }

        public un_builder ub()
        {
            if (isOracle()) return new un_builder_oracle();
            else return new un_builder();
        }

        public DBFile db()
        {
            if (isOracle()) return new DBFileOracle();
            else return new DBFile();
        }

        public DnFile dn()
        {
            if (isOracle()) return new DnFileOracle();
            else return new DnFile();
        }

        public DnFolder dnf()
        {
            if (isOracle()) return new DnFolderOracle();
            else return new DnFolder();
        }

        public fd_scan sa()
        {
            if (isOracle()) return new fd_scan_oracle();
            else return new fd_scan();
        }

        public SqlExec ec()
        {
            if (isOracle()) return new OracleExec();
            else return new SqlExec();
        }

        public DbBase bs()
        {
            if (isOracle()) return new OracleDbBase();
            else return new DbBase();
        }

    }
}