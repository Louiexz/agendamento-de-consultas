package com.example.unitsaude.data.api;

import com.example.unitsaude.data.dto.auth.LoginRequest;
import com.example.unitsaude.data.dto.auth.LoginResponse;
import com.example.unitsaude.data.dto.auth.RecuperarSenhaRequest;
import com.example.unitsaude.data.dto.auth.RecuperarSenhaResponse;
import com.example.unitsaude.data.dto.auth.RedefinirSenhaRequest;
import com.example.unitsaude.data.dto.auth.RegisterRequest;

import com.example.unitsaude.data.dto.consultation.GetConsultationRequest;
import com.example.unitsaude.data.dto.consultation.GetConsultationResponse;

import com.example.unitsaude.data.dto.consultation.CreateConsultationRequest;
import com.example.unitsaude.data.dto.consultation.CreateConsultationResponse;

import com.example.unitsaude.data.dto.consultation.GetSpecialtyResponse;

import com.example.unitsaude.data.dto.consultation.GetHourRequest;
import com.example.unitsaude.data.dto.consultation.GetHourResponse;

import com.example.unitsaude.data.dto.professor.GetProfessorResponse;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.Query;
import retrofit2.http.Path;
import retrofit2.http.POST;
import retrofit2.http.PATCH;
import retrofit2.http.GET;

public interface ApiService {

    @POST("/api/Usuario/Login")
    Call<LoginResponse> login(@Body LoginRequest loginRequest);

    @POST("/api/Usuario/recuperar-senha")
    Call<ResponseBody> recuperarSenha(@Body RecuperarSenhaRequest emailRequest);

    @POST("/api/Usuario/redefinir-senha")
    Call<ResponseBody> redefinirSenha(@Body RedefinirSenhaRequest request);

    @POST("/api/Paciente/CreatePaciente")
    Call<ResponseBody> registrar(@Body RegisterRequest registerRequest);

    @GET("/api/Consulta/GetConsultaPorPaciente/{pacienteId}")
    Call<GetConsultationResponse> getConsultas(@Path("pacienteId") int pacienteId);
    
    @GET("/api/Consulta/especialidades/{area}")
    Call<GetSpecialtyResponse> getSpecialities(@Path("area") String area);

    @GET("/api/Consulta/horarios-disponiveis")
    Call<GetHourResponse> getHours(
        @Query("data") String data,
        @Query("area") String area,
        @Query("especialidade") String especialidade);
    
    @GET("api/Professor/listar-professores-especialidade")
    Call<GetProfessorResponse> getProfessoresPorEspecialidade(@Query("especialidade") String especialidade);

    @POST("api/Consulta/CreateConsulta")
    Call<CreateConsultationResponse> createConsulta(@Body CreateConsultationRequest createConsultaRequest);

    @PATCH("api/Consulta/CancelarConsulta/{consultaId}")
    Call<Void> cancelConsultation(@Path("consultaId") int consultaId);
}
