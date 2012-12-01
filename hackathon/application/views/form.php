<html>
    <head>
        
        <title>Sign up !</title>
        
        <style type="text/css">
            ul{
                list-style: none;
                    
            }
        </style>
        
        
    </head>
    <body>
        <div id="form" class ="form" >
            <?php echo form_open('user/signup' , array("method" => 'post')); ?>
            <legend>Sign up</legend>
            
            <ul>
                
                <li>
                    <label for="user_name"> Family Name :</label>
                    <input type="text" placeholder="User Name" name="user_name" required="required" />
                </li>
                <li>
                    <label for="division_name"> Division Name :</label>
                    <select name="division_name">
                        <option value="dhaka">Dhaka</option>
                        <option value="rajshahi"> Rajshahi </option>
                        <option value="chittagong"> Chittagong</option>
                        <option value="sylhet">Sylhet</option>
                        <option value="rangpur"> Rangpur </option>
                        <option vlaue ="barisal"> Borishal </option>
                        <option value ="khulna"> Khulna </option>
                    </select>
                </li> <br/>
                <label for="pass"> Password :  </label>
                <input type="password" name="pass" required="required" />
                
                <li>
                    <input type="submit" value="submit" />
                </li>
            </ul>
            
            <?php echo form_close(); ?>
            
            <br/>
            <br/>
            
            <?php echo anchor('user/awerness_survey', "View Awareness related Survey") ?><br/>
            <?php echo anchor('user/usage_survey', "View Real Life Usage Data") ?>
            
        </div>
    </body>
    
</html>