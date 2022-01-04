var _lastClickTime;

async function initButton(empeName, currentTime) {

	$(".tbar").addClass("tbar-homescreen-mobile");
 
	$("#EpfEditGrid1").hide();

	$(".lblHeader").text("Welcome, "+empeName);
	DisplayTime(new Date(currentTime));
	
	var linkwrapper = $("#linkwrapper");
	if (linkwrapper.length == 0) {
		if (screen.availWidth < 768) {
			linkwrapper = $("<div id='linkwrapper' class='col-12 d-flex'></div>")
				.append("<a class='linksRedirect ml-auto mr-4 my-4' href='EpfScreen/TMS_ABS_REQUEST_BLZ_MOB?MODE=1'>Can't Report for Work</a>");
		}
		else {
			linkwrapper = $("<div id='linkwrapper' class='col-12 d-flex'></div>")
				.append("<a class='linksRedirect ml-auto mr-4 my-4' href='EpfScreen/TMS_ABS_REQUEST_BLZ_WEB'>Can't Report for Work</a>");
		}
		$("#EpfSubContainer19").append(linkwrapper);
	}
	

	if (document.getElementById("overlayOT")!==null){
		$("#overlayOT").remove();
	}
		
	$("#btnCalender").click(function(){             
		//   $("#C1").toggle("slow");
		//   $("#C3").toggle('slow');
		var overlayOT = $("<div>").attr("id","overlayOT");    
		overlayOT.addClass("sidenav-overlay-OT");
		  
		$( "#C4" ).show();
		$("#btnCalender").prop("disabled","true");
		if (screen.availWidth>768){
		$( "#C4" ).animate({ "top": "-=35vw" }, "slow" );
		}
		else{
		$( "#C4" ).animate({ "top": "-=115vw" }, "slow" );
		$("#TMS_HOMESCREEN_BS_BLZ").css("height","100vh");
		}
		$("body").append(overlayOT);
	});
	$("#btnClose").click(function(){      
		$("#btnCalender").prop("disabled","");   
		  
		if (screen.availWidth>768){
		$( "#TotalOTContainer" ).animate({ "top": "+=35vw" }, "slow" );
		$( "#TotalOTContainer" ).hide("slow");			
			
		}
		else{
		$( "#TotalOTContainer" ).animate({ "top": "+=115vw" }, "slow" );
		$( "#TotalOTContainer" ).hide("slow");			
		}     
		$("#overlayOT").remove();
	});

	document.getElementById('btnClock').addEventListener('click', function (e) {

		//[05/May/2021, PhyoZin] Check last click time and do nothing if user click again within 3 seconds
		if (!_lastClickTime) {
			_lastClickTime = new Date().getTime();
		}
		else {
			var currentTime = new Date().getTime();
			var clickInterval = currentTime - _lastClickTime;
			if (clickInterval < 3000) { // wait for 3 seconds
				return;
			}
			_lastClickTime = currentTime;
		}

	});


		
	document.getElementById("queryIcon").addEventListener('click',function(){			
		showClarification();
	});
	
	function showClarification(){
		var overlayOT = $("<div>").attr("id","overlayOT");    
		overlayOT.addClass("sidenav-overlay-OT");
		$("#TotalOTContainer").show();		
		$("#TMS_HOMESCREEN_BS_BLZ").css("height","100%");	
		if (screen.availWidth>768){
			$( "#TotalOTContainer" ).animate({ "top": "-=35vw" }, "slow" );
			}
			else{
			$( "#TotalOTContainer" ).animate({ "top": "-=115vw" }, "slow" );				
			}
			$("body").append(overlayOT);

	}
		
	
	$("#_mobileSideNav").on('show.bs.collapse', function () {
			$('html, body').css({
			overflow: 'hidden',
			height: '100%'
		});	
		$(".tbartext").css("z-index","-10");
	});
	
	$("#_mobileSideNav").on('hidden.bs.collapse', function () {
		$('html, body').css({
		overflow: 'auto',
		height: 'auto'
	});
	$(".tbartext").css("z-index","10000");
		
	document.getElementsByClassName("sidenav-overlay")[0].innerHTML = "<span style='font-size:25px;color: #797979;font-weight:bold; float:right;margin-right: 10%;margin-top:7%;'>Menu</span>";
	});

}


function DisplayTime(time) {
	if ($("#month-year-lbl").length == 0) {
		return;
	}

	var formattedTime = formatAMPM(time);
	$("#month-year-lbl").text(formattedTime);
	$("#month-year-lbl").css("white-space", "pre");

	setTimeout(function () {
		time.setSeconds(time.getSeconds() + 1);
		DisplayTime(time);
	}, 955);
}

function formatAMPM(date) {
	var hours = date.getHours();
	var minutes = date.getMinutes();
	//var seconds = date.getSeconds();
	var ampm = hours >= 12 ? 'PM' : 'AM';
	hours = hours % 12;
	hours = hours ? hours : 12; // the hour '0' should be '12'
	minutes = minutes < 10 ? '0' + minutes : minutes;
	var day = date.getDate();
	var day_name = date.toLocaleString('default', { weekday: 'short' });
	var month = date.toLocaleString('default', { month: 'long' });
	var strTime = hours + ':' + minutes + ' ' + ampm + '   ' + day_name + ',' + day + ' ' + month;
	return strTime;
}