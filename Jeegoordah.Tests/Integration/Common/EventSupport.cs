﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coypu;
using Jeegoordah.Core;

namespace Jeegoordah.Tests.Integration.Common
{
    class EventSupport
    {
        private readonly BrowserSession browser;

        public EventSupport(BrowserSession browser)
        {
            this.browser = browser;
        }

        public void VisitEventsList()
        {
            browser.VisitTest("#events");
        }

        /// <summary>
        /// We should be on events page
        /// </summary> 
        public IList<SnapshotElementScope> GetPastEvents()
        {
            return GetEvents("past");
        }

        /// <summary>
        /// We should be on events page
        /// </summary> 
        public IList<SnapshotElementScope> GetPendingEvents()
        {
            return GetEvents("pending");
        }

        /// <summary>
        /// We should be in event editor
        /// </summary> 
        public void FillEditor(TestEvent e)
        {
            browser.FillIn("Name").With(e.Name);
            browser.FillIn("StartDate").With(e.StartDate);
            browser.FillIn("Description").With(e.Description);
            foreach (var checkbox in GetActiveBroCheckboxes())
            {
                checkbox.Click();
            }
            foreach (string bro in e.Bros)
            {
                browser.ClickButton(bro);
            }
        }

        /// <summary>
        /// We should be on event editor
        /// </summary> 
        public IEnumerable<SnapshotElementScope> GetActiveBroCheckboxes()
        {
            return browser.FindAllCss("#eventBros label.btn.active");
        }

        private IList<SnapshotElementScope> GetEvents(string list)
        {
            return browser.FindAllCss("#event-list-{0}>div".F(list)).ToList();
        }

        /// <summary>
        /// We should be on events page
        /// </summary>        
        public TestEvent CreateEvent(TestEvent @event)
        {            
            browser.ClickButton("createEventButton");
            FillEditor(@event);
            browser.ClickButton("modalOkButton");
            browser.WaitForAjax();
            return @event;
        }

        public TestEvent OpenEventDetails(TestEvent @event)
        {
            VisitEventsList();
            CreateEvent(@event);
            GetPastEvents()[0].ClickLink(@event.Name);
            browser.WaitForAjax();
            return @event;
        }
    }
}
