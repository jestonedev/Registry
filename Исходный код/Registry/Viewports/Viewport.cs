﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Reporting;
using Registry.Viewport.Presenters;
using Registry.Viewport.SearchForms;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    public class Viewport: DockContent, IMenuController
    {
        protected IMenuCallback MenuCallback { get; set; }
        public bool Selected { get; set; }
        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        protected BindingSource GeneralBindingSource;
        protected DataModel GeneralDataModel;

        protected Presenter Presenter { get; set; }

        protected Viewport()
            : this(null, null)
        {
        }

        protected Viewport(Viewport viewport = null, IMenuCallback menuCallback = null, Presenter presenter = null)
        {
            if (viewport != null)
            {
                StaticFilter = viewport.StaticFilter;
                DynamicFilter = viewport.DynamicFilter;
                ParentRow = viewport.ParentRow;
                ParentType = viewport.ParentType;
            }
            MenuCallback = menuCallback;
            Presenter = presenter;
            if (presenter != null)
            {
                GeneralBindingSource = presenter.ViewModel["general"].BindingSource;
                GeneralDataModel = presenter.ViewModel["general"].Model;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e.Cancel != true)
            {
                RemoveAllEventHandlers();
                MenuCallback.SwitchToPreviousViewport();
            }
            base.OnClosing(e);
        }

        public virtual int GetRecordCount()
        {
            return GeneralBindingSource == null ? 0 : GeneralBindingSource.Count;
        }

        public virtual bool CanDuplicate()
        {
            return false;
        }

        public virtual bool CanLoadData()
        {
            return false;
        }

        public virtual void LoadData()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void MoveFirst()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void MovePrev()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void MoveNext()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void MoveLast()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual bool CanMoveFirst()
        {
            return false;
        }

        public virtual bool CanMovePrev()
        {
            return false;
        }

        public virtual bool CanMoveNext()
        {
            return false;
        }

        public virtual bool CanMoveLast()
        {
            return false;
        }

        public virtual void SaveRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void CancelRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void CopyRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void InsertRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void DeleteRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void OpenDetails()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void DataRefresh()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void SearchRecord(SearchFormType searchFormType)
        {
            throw new ViewportException("Не реализовано");
        }

        protected virtual Viewport ShowAssocViewport<T>(IMenuCallback menuCallback, 
            string staticFilter, DataRow parentRow, ParentTypeEnum parentType) where T : Viewport
        {
            if (menuCallback == null)
                throw new ViewportException("Не заданна ссылка на интерфейс menuCallback");
            var viewport = ViewportFactory.CreateViewport<T>(menuCallback);
            viewport.StaticFilter = staticFilter;
            viewport.ParentRow = parentRow;
            viewport.ParentType = parentType;
            if (((IMenuController) viewport).CanLoadData())
                ((IMenuController) viewport).LoadData();
            menuCallback.AddViewport(viewport);
            return viewport;
        }

        public virtual void ClearSearch()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual bool CanSaveRecord()
        {
            return false;
        }

        public virtual bool CanCancelRecord()
        {
            return false;
        }

        public virtual bool CanCopyRecord()
        {
            return false;
        }

        public virtual bool CanInsertRecord()
        {
            return false;
        }

        public virtual bool CanDeleteRecord()
        {
            return false;
        }

        public virtual bool CanOpenDetails()
        {
            return false;
        }

        public virtual bool CanSearchRecord()
        {
            return false;
        }

        public virtual bool AllowHousingMenuTab()
        {
            return false;
        }

        public virtual bool AllowRelatedWorkMenuTab()
        {
            return false;
        }

        public virtual bool AllowSocialRecruitmentMenuTab()
        {
            return false;
        }

        public virtual bool SearchedRecords()
        {
            return false;
        }

        public virtual bool CanFilterSocialFund()
        {
            return false;
        }

        public virtual bool CanFilterCommercialFundFund()
        {
            return false;
        }

        public virtual bool CanFilterSpecialFund()
        {
            return false;
        }

        public virtual bool CanFilterOtherFunds()
        {
            return false;
        }

        public virtual void ForceClose()
        {
            RemoveAllEventHandlers();
            Dispose();
        }

        public virtual bool ViewportDetached()
        {
            return ((ParentRow != null) && ((ParentRow.RowState == DataRowState.Detached) || (ParentRow.RowState == DataRowState.Deleted)));
        }

        public virtual void ShowAssocViewport<T>() where T : Viewport
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowAssocViewport<T>(ResettleEstateObjectWay way) where T : Viewport
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual bool HasAssocViewport<T>() where T : Viewport
        {
            return false;
        }

        public virtual bool HasAssocViewport<T>(ResettleEstateObjectWay way) where T : Viewport
        {
            return false;
        }
        public virtual void GenerateReport(ReporterType reporterType)
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual bool HasReport(ReporterType reporterType)
        {
            return false;
        }
       
        public virtual Viewport Duplicate()
        {
            return this;
        }

        public virtual void LocateEntityBy(string fieldName, object value)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (GeneralBindingSource != null) 
                GeneralBindingSource.Dispose();
            base.Dispose(disposing);
        }

        private readonly List<ViewportEventInfo> _weakEvents = new List<ViewportEventInfo>();

        protected void AddEventHandler<T>(object sender, string eventName, EventHandler<T> handler)
            where T: EventArgs
        {
            var eventInfo = sender.GetType().GetEvent(eventName);
            var d = Delegate.CreateDelegate(eventInfo.EventHandlerType, handler.Target, handler.Method);
            eventInfo.AddEventHandler(sender, d);
            _weakEvents.Add(new ViewportEventInfo
            {
                Handler = d,
                EventInfo = eventInfo,
                Sender = sender
            });
        }

        protected void RemoveAllEventHandlers()
        {
            foreach (var weakEvent in _weakEvents)
            {
                weakEvent.EventInfo.RemoveEventHandler(weakEvent.Sender, weakEvent.Handler);
            }
            _weakEvents.Clear();
        }

        protected BindingSource BindDataSource(string dataMember, object dataSource = null)
        {
            var ds = dataSource ?? DataStorage.DataSet;

            return new BindingSource
            {
                DataMember = dataMember,
                DataSource = ds
            };
        }

        protected BindingSource BindDataSource<T>(object dataSource = null)
            where T : Entity
        {
            var ds = dataSource ?? DataStorage.DataSet;
            var attribute = typeof(T).GetCustomAttributes(false).OfType<DataTableAttribute>().FirstOrDefault();
            if (attribute == null)
            {
                throw new ViewportException(string.Format("У класса {0} не указан атрибут DataTableAttribute", typeof(T)));
            }
            if (string.IsNullOrEmpty(attribute.Name))
            {
                throw new ViewportException(string.Format("Свойство Name атрибута DataTableAttribute у класса {0} отсутствует", typeof(T)));
            }
            return new BindingSource
            {
                DataMember = attribute.Name,
                DataSource = ds
            };
        }
    }
}
