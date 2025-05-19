package com.example.unitsaude.viewmodel;

import android.app.Application;
import android.util.Log;

import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import com.example.unitsaude.data.api.ApiService;
import com.example.unitsaude.data.api.ApiClient;

import com.example.unitsaude.data.dto.consultation.GetSpecialtyResponse;

import com.example.unitsaude.data.repositorio.ConsultationRepository;
import com.example.unitsaude.utils.SharedPreferencesManager;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SelectSpecialtyView extends AndroidViewModel {
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<GetSpecialtyResponse> specialtyResponseLiveData = new MutableLiveData<>();
    private final ConsultationRepository consultationRepository;

    public SelectSpecialtyView(Application application) {
        super(application);
        consultationRepository = new ConsultationRepository();
    }

    public LiveData<String> getErrorLiveData() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoadingLiveData() {
        return loadingLiveData;
    }

    public LiveData<GetSpecialtyResponse> getSpecialtyResponseLiveData() {
        return specialtyResponseLiveData;
    }

    public void getSpecialities(String area) {
        loadingLiveData.setValue(true);

        consultationRepository.getSpecialities(area)
            .enqueue(new Callback<GetSpecialtyResponse>() {
                @Override
                public void onResponse(Call<GetSpecialtyResponse> call, Response<GetSpecialtyResponse> response) {
                    loadingLiveData.setValue(false);
                    if (response.isSuccessful() && response.body() != null) {
                        handleSuccessfulSpecialities(response.body());
                    } else {
                        handleSpecialitiesError(response);
                    }
                }

                @Override
                public void onFailure(Call<GetSpecialtyResponse> call, Throwable t) {
                    loadingLiveData.setValue(false);
                    errorLiveData.setValue("Erro ao conectar com o servidor");
                    Log.e("SPECIALTY_ERROR", "Erro ao conectar com o servidor", t);
                }
            });
    }


    private void handleSuccessfulSpecialities(GetSpecialtyResponse body) {
        specialtyResponseLiveData.setValue(body);
    }

    private void handleSpecialitiesError(Response<GetSpecialtyResponse> response) {
        String errorBody = "";
        try {
            if (response.errorBody() != null) {
                errorBody = response.errorBody().string();
            }
        } catch (Exception e) {
            errorBody = "Erro ao ler o corpo de erro: " + e.getMessage();
        }
        Log.e("SPECIALTY_FAIL", "Código HTTP: " + response.code() + ", Erro: " + errorBody);
        errorLiveData.setValue("Erro ao consultar: " + errorBody);
    }
}
