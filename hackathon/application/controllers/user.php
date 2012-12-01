<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of user
 *
 * @author insane
 */
class User extends CI_Controller {
    //put your code here
    function __construct() {
        parent::__construct();
    }
    
    
    function index(){
        redirect('user/signup');
    }
    
    
    
    function view(){
        $this->load->model("user_model");
        echo $this->user_model->response();
    }
    
    
    public function signup(){
        if($this->input->post()){
            $this->load->model("user_model");
            if($this->user_model->add_user()){
                $data ['messege'] = "your signup has been successful";
            }else{
                $data ['messege'] = "signup failed";
            }
            $this->load->view("notification" , $data);
        }else{
            $this->load->view("form");
        }
    }
    
    
    public function awerness_survey(){
        $this->load->model("user_model");
        $survay = $this->user_model->awareness_servey();
        $data = array();
        foreach ($survay as $key => $value) {
            if($value <35){
                $data[$key] = '/Red';
            }else if($value < 90){
                $data[$key] = '/Orange';
            }else{
                $data[$key] = '/Green';
            }
        }
        

        $data['title'] = "Awareness Survey Data Vizualization";
        $data['heading'] = "Awareness Survey Data Vizualization";
        $data['time'] = 2004;
        $this->load->view('data_view' , $data);
    }
    
    
    public function usage_survey(){
        $data['dhaka'] = '/Green';
        $data['chittagong'] = '/Orange';
        $data['rajshahi'] = "/Red";
        $data['rangpur'] = "/Red";
        $data['sylhet'] = '/Orange';
        $data['khulna'] = '/Orange';
        $data['barisal'] = "/Orange";
        $data['title'] = "Usage Survey Data Vizualization";
        $data['heading'] = "Usage Survey Data Vizualization";
        $data['time'] = 2004;
        $this->load->view('data_view' , $data);        
    }
    
    public function test(){
        
        var_dump($data);
    }
    
    
    public function families(){
        $this->load->model("auth_model");
        $family_list = $this->main_model->family_list();
        
    }
    
    
    
}



?>
