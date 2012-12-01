<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of user_model
 *
 * @author insane
 */
class User_model extends CI_Model{
    //put your code here
    function __construct() {
        parent::__construct();
    }
    
    function response(){
        return  "this is a response";
    }
    
    
    function add_user(){
        $signup_data = $this->input->post();
        $data['user_name'] = $signup_data['user_name'];
        $data['division_name'] = $signup_data['division_name'];
        $data['pass'] = $signup_data['pass'];
        return $this->db->insert('users' , $data);
    }
    
    function awareness_servey(){
        $data['khulna'] = $this->average_score_of_district('khulna');
        $data['dhaka'] = $this->average_score_of_district('dhaka');
        $data['rajshahi'] = $this->average_score_of_district('rajshah');
        $data['rangpur'] = $this->average_score_of_district('rangpur');
        $data['barisal'] = $this->average_score_of_district('barisal');
        $data['chittagong'] = $this->average_score_of_district('chittagong');
        $data['sylhet'] = $this->average_score_of_district('sylhet');
        return $data;
    }
    
    
    function usage_survey_list(){
        
    }
    
    
    
    




    function average_score_of_district($district){
        $query = $this->db->query('SELECT score FROM game join users on users.id = game.user_id where users.division_name ="'.$district.'"');
        $val = 0;
        
        
        foreach($query->result() as $row){
            $val = $val + $row->score;
        }
        if($query->num_rows() > 0){
            return  $val/$query->num_rows();
        }else{
            return 0;
        }
    }







    function familyLists(){
        $query = $this->db->get('users');
    }
    
}

?>
