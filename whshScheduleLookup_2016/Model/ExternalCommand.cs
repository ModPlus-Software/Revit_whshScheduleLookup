using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ModPlusAPI.Windows;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace whshScheduleLookup.Model
{
    public class ExternalCommand : IExternalEventHandler
    {
        private Action _doAction;
        private Action<object> _postAction;
        private Func<object> _doFunc;
        private Document _doc;
        private readonly ExternalEvent _exEvent;
        private string _tName;

        public ExternalCommand()
        {
            _exEvent = ExternalEvent.Create(this);
        }

        public void SetAction(Action doAction, string tName = "ModPlus_PluginEvent", Document doc = null)
        {
            _doAction = doAction;
            _doFunc = null;
            _doc = doc;
            _tName = tName;
            _exEvent.Raise();
        }

        public void SetFunction(Func<object> doFunc, Action<object> postAction, string tName = "ModPlus_PluginEvent", Document doc = null)
        {
            _doAction = null;
            _doFunc = doFunc;
            _postAction = postAction;
            _doc = doc;
            _tName = tName;
            _exEvent.Raise();
        }

        public void Execute(UIApplication app)
        {
            if (_doAction != null)
            {
                if (_doc == null) _doc = app.ActiveUIDocument.Document;
                //using (Transaction t = new Transaction(doc, tName))
                //{
                //    t.Start();
                    try
                    {
                        _doAction();
                        //t.Commit();
                    }
                    catch (OperationCanceledException)
                    {
                        //t.RollBack();
                        MessageBox.Show("Внимание!", "Операция была прервана!\nИзменения отменены.", MessageBoxIcon.Alert);
                    }
                //}
            }
            _doAction = null;
            if (_doFunc != null)
            {
                object result = null;
                if (_doc == null) _doc = app.ActiveUIDocument.Document;

                using (Transaction t = new Transaction(_doc, _tName))
                {
                    t.Start();
                    try
                    {
                        result = _doFunc();
                        t.Commit();
                    }
                    catch (OperationCanceledException)
                    {
                        t.RollBack();
                        MessageBox.Show("Внимание!", "Операция была прервана!\nИзменения отменены.", MessageBoxIcon.Alert);
                    }
                }
                _postAction(result);
            }
            _doFunc = null;
        }

        public string GetName()
        {
            return "PluginEvent";
        }
    }
}