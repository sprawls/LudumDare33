/// Copyright (C) 2012-2014 Soomla Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///      http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.

using System;
using System.Collections.Generic;

namespace Soomla.Store {

    /// <summary>
    /// This class defines our game's economy, which includes virtual goods, virtual currencies
    /// and currency packs, virtual categories
    /// </summary>
    public class EntropyAssets : IStoreAssets {

        /// <summary>
        /// see parent.
        /// </summary>
        public int GetVersion() {
            return 0;
        }

        /// <summary>
        /// see parent.
        /// </summary>
        public VirtualCurrency[] GetCurrencies() {
            return new VirtualCurrency[] { World1_CompletedLevels, World2_CompletedLevels };
        }

        /// <summary>
        /// see parent.
        /// </summary>
        public VirtualGood[] GetGoods() {
            return new VirtualGood[] { PREMIUM_LTVG, REFUNDTEST_LTVG, CANCELTEST_LTVG, TESTBUY_LTVG };
        }

        /// <summary>
        /// see parent.
        /// </summary>
        public VirtualCurrencyPack[] GetCurrencyPacks() {
            return new VirtualCurrencyPack[] { };
        }

        /// <summary>
        /// see parent.
        /// </summary>
        public VirtualCategory[] GetCategories() {
            return new VirtualCategory[] { GENERAL_CATEGORY };
        }

        /** Static Final Members **/

        public const string PREMIUM_VERSION_ID = "premium_version";

        public const string Levels_W1 = "levels_w1";
        public const string Levels_W2 = "levels_w2";

        public const string TEST_REFUND_ID = "android.test.refunded";
        public const string TEST_CANCEL_ID = "android.test.canceled";
        public const string TEST_PURCHASED_ID = "android.test.purchased";



        /** Virtual Currencies **/
        public static VirtualCurrency World1_CompletedLevels = new VirtualCurrency(
          "W1_CompletedLevels",                         // Name
          "Amount of completed levels in World 1",     // Description
          "levels_w1"                               // Item ID
        );

        public static VirtualCurrency World2_CompletedLevels = new VirtualCurrency(
          "W2_CompletedLevels",                         // Name
          "Amount of completed levels in World 2",     // Description
          "levels_w2"                               // Item ID
        );

        /** Virtual Currency Packs **/

        /** Virtual Goods **/

        /** Virtual Categories **/
        // The muffin rush theme doesn't support categories, so we just put everything under a general category.
        public static VirtualCategory GENERAL_CATEGORY = new VirtualCategory(
                "General", new List<string>(new string[] { PREMIUM_VERSION_ID, TEST_REFUND_ID, TEST_CANCEL_ID, TEST_PURCHASED_ID })
        );


        /** LifeTimeVGs **/
        // Note: create non-consumable items using LifeTimeVG with PuchaseType of PurchaseWithMarket
        public static VirtualGood PREMIUM_LTVG = new LifetimeVG(
            "Unlock Full Game", 											// name
            "Unlocks the full game and removes the ads !",				 	// description
            "premium_version",												// item id
            new PurchaseWithMarket(PREMIUM_VERSION_ID, 0.99));	            // the way this virtual good is purchased

        public static VirtualGood REFUNDTEST_LTVG = new LifetimeVG(
            "Refund Test", 											        // name
            "Tests the refund",				 	                            // description
            "android.test.refunded",										// item id
            new PurchaseWithMarket(TEST_REFUND_ID, 0.99));	                // the way this virtual good is purchased

        public static VirtualGood CANCELTEST_LTVG = new LifetimeVG(
            "Cancel Test", 													// name
            "Tests the cancel",				 								// description
            "android.test.canceled",										// item id
            new PurchaseWithMarket(TEST_CANCEL_ID, 0.99));                  // the way this virtual good is purchased

        public static VirtualGood TESTBUY_LTVG = new LifetimeVG(
            "Buy Test", 														// name
            "Tests the Buy",				 									// description
            "android.test.purchased",														// item id
            new PurchaseWithMarket(TEST_PURCHASED_ID, 0.99));	// the way this virtual good is purchased

    }
}

