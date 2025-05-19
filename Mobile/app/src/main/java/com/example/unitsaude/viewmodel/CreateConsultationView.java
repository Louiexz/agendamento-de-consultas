package com.example.unitsaude.viewmodel;

import android.app.Application;
import android.util.Log;

import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import com.example.unitsaude.data.api.ApiService;
import com.example.unitsaude.data.api.ApiClient;

import com.example.unitsaude.data.dto.consultation.CreateConsultationResponse;
import com.example.unitsaude.data.dto.consultation.CreateConsultationRequest;

import com.example.unitsaude.data.repositorio.ConsultationRepository;
import com.example.unitsaude.utils.SharedPreferencesManager;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;


public class CreateConsultationView extends AndroidViewModel {
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<CreateConsultationResponse> consultationResponseLiveData = new MutableLiveData<>();
    private final ConsultationRepository consultationRepository;

    public CreateConsultationView(Application application) {
        super(application);
        consultationRepository = new ConsultationRepository();
    }

    public LiveData<String> getErrorLiveData() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoadingLiveData() {
        return loadingLiveData;
    }

    public LiveData<CreateConsultationResponse> getConsultationResponseLiveData() {
        return consultationResponseLiveData;
    }

    public void createConsulta(CreateConsultationRequest createConsultaRequest) {
        loadingLiveData.setValue(true);

        consultationRepository.createConsulta(createConsultaRequest)
            .enqueue(new Callback<CreateConsultationResponse>() {
                @Override
                public void onResponse(Call<CreateConsultationResponse> call, Response<CreateConsultationResponse> response) {
                    loadingLiveData.setValue(false);
                    if (response.isSuccessful() && response.body() != null) {
                        handleSuccessfulConsultation(response.body());
                    } else {
                        handleConsultationError(response);
                    }
                }

                @Override
                public void onFailure(Call<CreateConsultationResponse> call, Throwable t) {
                    loadingLiveData.setValue(false);
                    errorLiveData.setValue("Erro ao conectar com o servidor");
                    Log.e("CONSULTATION_ERROR", "Erro ao conectar com o servidor", t);
                }
            });
    }

    private void handleSuccessfulConsultation(CreateConsultationResponse body) {
        consultationResponseLiveData.setValue(body);
    }

    private void handleConsultationError(Response<CreateConsultationResponse> response) {
        String errorBody = "";
        try {
            if (response.errorBody() != null) {
                errorBody = response.errorBody().string();
            }
        } catch (Exception e) {
            errorBody = "Erro ao ler o corpo de erro: " + e.getMessage();
        }
        Log.e("CONSULTATION_FAIL", "Código HTTP: " + response.code() + ", Erro: " + errorBody);
        errorLiveData.setValue("Erro ao consultar: " + errorBody);
    }
}