﻿using Core.Domain;
using Core.Domain.Repository;
using BTCSTXSwap.Domain.Interfaces.Core;
using BTCSTXSwap.Domain.Interfaces.Factory.GLog;
using BTCSTXSwap.Domain.Interfaces.Models.GLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCSTXSwap.Domain.Impl.Models.GLog
{
    public class GLogModel: IGLogModel
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGLogDomainFactory _glogFactory;
        private readonly IGLogRepository<IGLogModel, IGLogDomainFactory> _repLog;

        public GLogModel(
            ILogCore log,
            IUnitOfWork unitOfWork,
            IGLogDomainFactory glogFactory,
            IGLogRepository<IGLogModel, IGLogDomainFactory> repLog
        )
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _glogFactory = glogFactory;
            _repLog = repLog;
        }

        public long IdLog { get; set; }
        public long IdUser { get; set; }
        public string Ip { get; set; }
        public DateTime InsertDate { get; set; }
        public string Message { get; set; }
        public string LogType { get; set; }

        public IEnumerable<IGLogModel> List(long idUser, int page, out int balance)
        {
            return _repLog.List(_glogFactory, idUser, page, out balance);
        }
        public void Insert()
        {
            _repLog.Insert(this);
        }
    }
}
