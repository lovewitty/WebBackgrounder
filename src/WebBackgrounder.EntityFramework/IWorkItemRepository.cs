﻿using System;
using WebBackgrounder.EntityFramework.Entities;

namespace WebBackgrounder.EntityFramework
{
    public interface IWorkItemRepository
    {
        void RunInTransaction(Action query);
        bool AnyActiveWorker { get; }
        object CreateWorkItem(string workerId);
        void SetWorkItemCompleted(object workItemId);
        void SetWorkItemFailed(object workItemId, Exception exception);
    }
}
