package com.example.unitsaude.viewmodel;

import android.app.Application;
import android.util.Log;

import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import com.example.unitsaude.data.api.ApiService;
import com.example.unitsaude.data.api.ApiClient;

import com.example.unitsaude.data.dto.consultation.GetConsultationResponse;

import com.example.unitsaude.data.repositorio.ConsultationRepository;
import com.example.unitsaude.utils.SharedPreferencesManager;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class CancelConsultationView extends AndroidViewModel {
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<String> consultationResponseLiveData = new MutableLiveData<>();
    private final ConsultationRepository consultationRepository;
    private final SharedPreferencesManager preferencesManager;

    public CancelConsultationView(Application application) {
        super(application);
        consultationRepository = new ConsultationRepository();
        preferencesManager = SharedPreferencesManager.getInstance(application);
    }

    public LiveData<String> getErrorLiveData() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoadingLiveData() {
        return loadingLiveData;
    }

    public LiveData<String> getConsultationResponseLiveData() {
        return consultationResponseLiveData;
    }

    public void cancelConsultation(int consultaId) {
        loadingLiveData.setValue(true);

        consultationRepository.cancelConsultation(consultaId).enqueue(new Callback<Void>() {
            @Override
            public void onResponse(Call<Void> call, Response<Void> response) {
                loadingLiveData.setValue(false);
                if (response.isSuccessful()) {
                    consultationResponseLiveData.setValue("Consulta cancelada com sucesso");
                } else {
                    handleError(response);
                }
            }

            @Override
            public void onFailure(Call<Void> call, Throwable t) {
                loadingLiveData.setValue(false);
                errorLiveData.setValue("Erro ao conectar com o servidor");
                Log.e("CANCEL_CONSULTATION", "Erro ao conectar com o servidor", t);
            }
        });
    }

    private void handleError(Response<Void> response) {
        String errorBody = "";
        try {
            if (response.errorBody() != null) {
                errorBody = response.errorBody().string();
            }
        } catch (Exception e) {
            errorBody = "Erro ao ler o corpo de erro: " + e.getMessage();
        }
        Log.e("CANCEL_CONSULTATION_FAIL", "Código HTTP: " + response.code() + ", Erro: " + errorBody);
        errorLiveData.setValue("Erro ao cancelar consulta: " + errorBody);
    }
}
