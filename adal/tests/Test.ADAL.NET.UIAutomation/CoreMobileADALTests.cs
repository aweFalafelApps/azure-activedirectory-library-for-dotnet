﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Test.Microsoft.Identity.Core.UIAutomation.infrastructure;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Test.ADAL.NET.UIAutomation
{
    /// <summary>
    /// Contains the core test functionality that will be used by Android and iOS tests
    /// </summary>
	public class CoreMobileADALTests
    {
        private const string UiAutomationTestClientId = "3c1e0e0d-b742-45ba-a35e-01c664e14b16";
        private const string MSIDLAB4ClientId = "4b0db8c2-9f26-4417-8bde-3f0e3656f8e0";
        private const string MSGraph = "https://graph.microsoft.com";
        private const string UiAutomationTestResource = "ae55a6cc-da5e-42f8-b75d-c37e41a1a0d9";

        /// <summary>
        /// Runs through the standard acquire token interactive flow
        /// </summary>
        /// <param name="controller">The test framework that will execute the test interaction</param>
        public static void AcquireTokenInteractiveTest(ITestController controller)
		{
            //Get User from Lab
            var user = controller.GetUser(
                new UserQueryParameters
                {
                    IsMamUser = false,
                    IsMfaUser = false,
                    IsFederatedUser = false
                });

            controller.Tap("secondPage");

            //Clear Cache
            controller.Tap("clearCache");

            //Enter ClientID
            controller.EnterText("clientIDEntry", UiAutomationTestClientId, false);

            //Enter Resource
            controller.EnterText("resourceEntry", MSGraph, false);

            //Acquire token flow
            controller.Tap("acquireToken");
            //i0116 = UPN text field on AAD sign in endpoint
            controller.EnterText("i0116", user.Upn, true);
            //idSIButton9 = Sign in button
            controller.Tap("idSIButton9", true);
            //i0118 = password text field
            controller.EnterText("i0118", ((LabUser)user).GetPassword(), true);
            controller.Tap("idSIButton9", true);

            //Verify result. Test results are put into a label
            Assert.IsTrue(controller.GetText("testResult") == "Success: True");
        }

        /// <summary>
        /// Runs through the standard acquire token silent flow
        /// </summary>
        /// <param name="controller">The test framework that will execute the test interaction</param>
        public static void AcquireTokenSilentTest(ITestController controller)
        {
            //Get User from Lab
            var user = controller.GetUser(
                new UserQueryParameters
                {
                    IsMamUser = false,
                    IsMfaUser = false,
                    IsFederatedUser = false
                });

            controller.Tap("secondPage");

            //Clear Cache
            controller.Tap("clearCache");

            //Acquire token flow
            controller.Tap("acquireToken");
            //i0116 = UPN text field on AAD sign in endpoint
            controller.EnterText("i0116", user.Upn, true);
            //idSIButton9 = Sign in button
            controller.Tap("idSIButton9", true);
            //i0118 = password text field
            controller.EnterText("i0118", ((LabUser)user).GetPassword(), true);
            controller.Tap("idSIButton9", true);

            //Verify result. Test results are put into a label
            Assert.IsTrue(controller.GetText("testResult") == "Success: True");
        }
    }
}