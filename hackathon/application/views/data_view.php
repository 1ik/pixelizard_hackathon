<html>
    <head>
        <link href='http://fonts.googleapis.com/css?family=Sanchez' rel='stylesheet' type='text/css'>
        <link href='http://fonts.googleapis.com/css?family=Sanchez|Croissant+One' rel='stylesheet' type='text/css'>
        <link rel="stylesheet" type="text/css" href="<?php echo site_url('css/style.css'); ?>"/>
        <title><?php if(isset($title)) echo $title; ?></title>
    </head>
    
    
    
    <body>
        <h1 style="font-family: 'Sanchez', serif;" > <?php if(isset($heading)) echo $heading; ?></h1>
        <h2 style="font-family: 'Sanchez', serif;" ><?php if(isset($time)) echo $time ; ?></h2>
        
        <div class="info">
            <span style="color: #99BB66">Good</span> | <span style="color: #F47920">Fair</span> | <span style="color: #CC3333">Bad</span>
        </div>
            
        <div style="position: absolute" class="map">
            <img src="<?php echo site_url('images'); ?><?php echo $barisal; ?>/barisal.png" class="img1" />
            <img src="<?php echo site_url('images'); ?><?php echo $chittagong; ?>/chittagong.png" class="img2" />
            <img src="<?php echo site_url('images'); ?><?php echo $dhaka ; ?>/dhaka.png" class="img2" />
            <img src="<?php echo site_url('images'); ?><?php echo $rajshahi ; ?>/rajshahi.png" class="img2" />
            <img src="<?php echo site_url('images'); ?><?php echo $sylhet ; ?>/sylhet.png" class="img2" />
            <img src="<?php echo site_url('images'); ?><?php echo $khulna ; ?>/khulna.png" class="img2" />
            <img src="<?php echo site_url('images'); ?><?php echo $rangpur ; ?>/rangpur.png" class="img2" />
        </div>
        
        
        
    </body>
</html>