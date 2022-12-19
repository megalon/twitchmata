using System.Collections;
using System.Collections.Generic;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomRewardRedemptionStatus;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses.Messages.Redemption;
using TwitchLib.Unity;
using UnityEngine;
using System.Threading;
using UnityEngine.Events;

namespace Twitchmata {
    public class ChannelPointManager : FeatureManager {
        //MARK: - API Setup

        override internal void InitializePubSub(PubSub pubSub) {
            Debug.Log("Setting up Channel Points");
            pubSub.OnChannelPointsRewardRedeemed -= PubSub_OnChannelPointsRewardRedeemed;
            pubSub.OnChannelPointsRewardRedeemed += PubSub_OnChannelPointsRewardRedeemed;
            pubSub.ListenToChannelPoints(this.ChannelID);
        }

        private void PubSub_OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e) {
            var redemption = e.RewardRedeemed.Redemption;
            if (redemption.Status == "UNFULFILLED") {
                this.manager.api.InvokeAsync(this.HelixAPI.ChannelPoints.UpdateRedemptionStatusAsync(e.ChannelId, redemption.Id, new List<string>() { redemption.Id }, new UpdateCustomRewardRedemptionStatusRequest() { Status = CustomRewardRedemptionStatus.FULFILLED }));
            } else if (redemption.Status == "FULFILLED") {
                Debug.Log($"FULFILLED: {redemption.User.DisplayName}, {redemption.Reward.Title}");
            }

            this.ChannelPointsRedeemed(e.RewardRedeemed);
        }

        #region Notifications
        /// <summary>
        /// Called when a user redeems a reward using channel points
        /// </summary>
        /// <param name="reward">The reward that was redeemed</param>
        public virtual void ChannelPointsRedeemed(RewardRedeemed reward) {
            Debug.Log($"User redeemed {reward.Redemption.Reward.Title}");
        }

        #endregion
    }
}

