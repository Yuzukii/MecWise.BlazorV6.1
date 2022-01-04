 $(function () {
     $(document).keydown(function (e) {
        switch (e.which) {
            case 13: // enter key
                var userID = document.getElementById("UserID");
                if (!userID) { return; }
                var userPassword = document.getElementById("UserPassword");
                if (!userPassword) { return; }

                if (userID.value != '' || userPassword.value != '') {
                    $("#btnLogin").focus();
                    $("#btnLogin").click();
                    break;
                }
        }
     });
})


  
