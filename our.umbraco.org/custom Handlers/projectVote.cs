﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using uPowers.BusinessLogic;
using umbraco.cms.businesslogic.web;

namespace our.custom_Handlers {
    public class projectVoteHandler : umbraco.BusinessLogic.ApplicationBase {

        public projectVoteHandler(){
            uPowers.BusinessLogic.Action.BeforePerform += new EventHandler<ActionEventArgs>(ProjectVote);
            uPowers.BusinessLogic.Action.AfterPerform += new EventHandler<ActionEventArgs>(Action_AfterPerform);
        }

        void Action_AfterPerform(object sender, ActionEventArgs e)
        {
            uPowers.BusinessLogic.Action a = (uPowers.BusinessLogic.Action)sender;

            if (a.Alias == "ProjectUp")
            {
                Document d = new Document(e.ItemId);

                if (d.getProperty("approved").Value != null &&
                     d.getProperty("approved").Value.ToString() != "1" &&
                     uPowers.Library.Xslt.Score(d.Id, "powersProject") >= 15)
                {
                    //set approved flag
                    d.getProperty("approved").Value = true;

                    d.Save();
                    d.Publish(new umbraco.BusinessLogic.User(0));

                    umbraco.library.UpdateDocumentCache(d.Id);
                    umbraco.library.RefreshContent();
                }
            }
        }

        void ProjectVote(object sender, ActionEventArgs e) {
            uPowers.BusinessLogic.Action a = (uPowers.BusinessLogic.Action)sender;

            if (a.Alias == "ProjectUp" || a.Alias == "ProjectDown") {

                Document d = new Document(e.ItemId);

                e.ReceiverId = (int)d.getProperty("owner").Value;

                e.ExtraReceivers = Utills.GetProjectContributors(d.Id);
            }
        }
    }
}
