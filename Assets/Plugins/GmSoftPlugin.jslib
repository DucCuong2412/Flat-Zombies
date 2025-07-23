var functions = {
	SDK_Init: function(_enableAds,_gameKey,_pubID,_debug,_unlockTimer,_timeShowInter,_sdkType) {
			//FIX ME
		
	}, 
	
	SDK_GMEvent: function(eventName, msg)
	{
		gmEvent(UTF8ToString(eventName), UTF8ToString(msg));
	},
   
	SDK_InitParam: function() {
		let hostname = document.location.hostname;
		SendMessage("GmSoft", "SetUnityHostName", hostname);
		SendMessage("GmSoft", "SetParam",JSON.stringify(window["GMSOFT_OPTIONS"])); 
	}, 
	SDK_PreloadAd: function() {
	if(window["GMSOFT_OPTIONS"].enableAds==true){ 
		if(window["GMSOFT_OPTIONS"].sdkType=="wgplayer"){ 
			//init/prepare rewarded ad
			console.log( "Setup rewarded ads called" ); 
			window[window.preroll.config.loaderObjectName].registerRewardCallbacks({
				onReady:function(response){ 
					SendMessage("GmSoft", "PreloadRewardedVideoCallback",1);
					this._showRewardAdFn = showAdFn;
					console.log( "The rewarded ad is ready. " );
				},//called when a rewarded ad is ready to be displayed
				onSuccess:function(response){ 
					this._showRewardAdFn = null;
					SendMessage("GmSoft", "RewardedVideoSuccessCallback");
					console.log( "The rewarded ad has been succesfully displayed, you can now grand the reward to user." );
				},//the ad is finished, user should be rewarded with the prize
				onFail:function(response){ 
					this._showRewardAdFn = null;
					SendMessage("GmSoft", "RewardedVideoFailureCallback");
					console.log( "The user did not go thrught" );
				}//the ad dispaly was canceled, user should not be rewarded with the prize
			});
		 
			
			
		}else if(window["GMSOFT_OPTIONS"].sdkType=="h5"){
			// Check if rewarded ad is available to view
			if (afg.ready) {
				 SendMessage("GmSoft", "PreloadRewardedVideoCallback",1);
				 afg.adBreak({
					type: 'reward',
					name: 'reward ads',
					beforeReward: function(showAdFn) { 
						this._showRewardAdFn = showAdFn;
						console.log("before reward");
					}.bind(this),
					adViewed: function() { 
						this._showRewardAdFn = null;
						SendMessage("GmSoft", "RewardedVideoSuccessCallback");
						console.log("ad viewed");
					}.bind(this),
					adDismissed: function() { 
						this._showRewardAdFn = null;
						SendMessage("GmSoft", "RewardedVideoFailureCallback");
						console.log("ad failure");
					}.bind(this),
					adBreakDone: function(placementInfo) { 
						console.log("ad break done");
						console.log("reward break done");
						SendMessage("GmSoft", "ResumeGameCallback");
					}.bind(this)
				}); 
			}else{
				 console.log("no reward ads");
				 SendMessage("GmSoft", "PreloadRewardedVideoCallback",0);
				 SendMessage("GmSoft", "ResumeGameCallback");
			}
		}		
	}else{
		 SendMessage("GmSoft", "ResumeGameCallback");
	}
  },

  SDK_ShowAd: function(adType) {
	
	if(window["GMSOFT_OPTIONS"].enableAds==true){ 
		adType = UTF8ToString(adType);
		var _ad_type = ["preroll","start","pause","next","midroll","browse","reward","preload-reward"];
		if ((_ad_type.indexOf(adType) > -1)==false) adType = "start";
		
		console.log("adType ===> :"+adType);
		console.log("GMSOFT_OPTIONS.sdkType ===> :"+GMSOFT_OPTIONS.sdkType);
		
		if(adType=="start" || adType=="preroll" || adType=="next" || adType=="midroll" ){ 
			
			if(window["GMSOFT_OPTIONS"].sdkType=="wgplayer"){
					if(adType=="start" || adType=="preroll"){
						window[preroll.config.loaderObjectName].fetchAd(
							function(response){ 
								SendMessage("GmSoft", "ResumeGameCallback"); 
								console.log( "Afg preroll has been displayed and is now finished, we cand now load the game" );
							}
						);
					}else if(adType=="midroll" || adType=="next"){
						window[preroll.config.loaderObjectName].refetchAd(
							function(response){ 
								SendMessage("GmSoft", "ResumeGameCallback"); 
								console.log( "Afg preroll has been displayed and is now finished, we cand now load the game" );
							}
						);
					}
			}else if(window["GMSOFT_OPTIONS"].sdkType=="h5"){
				if (afg.ready) {
					afg.adBreak({
						type: adType,
						name: adType,
						beforeAd: function () {
							afg.onBeforeAd();
							console.log("before ad");
							SendMessage("GmSoft", "PauseGameCallback");
						}.bind(this),
						adBreakDone: function () { 
							 console.log("Preroll done viewed");
							 SendMessage("GmSoft", "ResumeGameCallback");
						}.bind(this)
					});
				}
				else
				{
					console.log("no "+adType+" ads");
					SendMessage("GmSoft", "ResumeGameCallback");
				}
			}
			
					
		} else if(adType=="reward"){
			if(window["GMSOFT_OPTIONS"].sdkType=="wgplayer"){
				if(this._showRewardAdFn) {
					SendMessage("GmSoft", "PauseGameCallback"); 
					window[window.preroll.config.loaderObjectName].showRewardAd();  
				}
					
			}else if(window["GMSOFT_OPTIONS"].sdkType=="h5"){
				if (this._showRewardAdFn) {
					SendMessage("GmSoft", "PauseGameCallback");
					this._showRewardAdFn();			
				} 
			}
				
		} 
	}else{
		 SendMessage("GmSoft", "ResumeGameCallback");
	}
  },

  SDK_SendEvent : function(options) {
    options = UTF8ToString(options);
    if (typeof gdsdk !== "undefined" && typeof gdsdk.sendEvent !== "undefined" && typeof options !== "undefined") {
      gdsdk.sendEvent(options)
      .then(function(response){
       
        console.log("Game event post message sent Succesfully...")
        
      })
      .catch(function(error){
        console.log(error.message)
      });
    }
  }
};

mergeInto(LibraryManager.library, functions);