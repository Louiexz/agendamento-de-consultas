package com.example.unitsaude.viewmodel;

import android.app.Application;
import android.util.Log;

import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import com.example.unitsaude.data.api.ApiService;
import com.example.unitsaude.data.api.ApiClient;

import com.example.unitsaude.data.dto.consultation.GetHourResponse;
import com.example.unitsaude.data.dto.consultation.GetHourRequest;

import com.example.unitsaude.data.repositorio.ConsultationRepository;
import com.example.unitsaude.utils.SharedPreferencesManager;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SelectHourView extends AndroidViewModel {
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<GetHourResponse> hourResponseLiveData = new MutableLiveData<>();
    private final ConsultationRepository consultationRepository;

    public SelectHourView(Application application) {
        super(application);
        consultationRepository = new ConsultationRepository();
    }

    public LiveData<String> getErrorLiveData() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoadingLiveData() {
        return loadingLiveData;
    }

    public LiveData<GetHourResponse> getHourResponseLiveData() {
        return hourResponseLiveData;
    }

    public void getHours(GetHourRequest request) {
        loadingLiveData.setValue(true);

        consultationRepository.getHours(request)
            .enqueue(new Callback<GetHourResponse>() {
                @Override
                public void onResponse(Call<GetHourResponse> call, Response<GetHourResponse> response) {
                    loadingLiveData.setValue(false);
                    if (response.isSuccessful() && response.body() != null) {
                        handleSuccessfulHours(response.body());
                    } else {
                        handleHoursError(response);
                    }
                }

                @Override
                public void onFailure(Call<GetHourResponse> call, Throwable t) {
                    loadingLiveData.setValue(false);
                    errorLiveData.setValue("Erro ao conectar com o servidor");
                    Log.e("HOUR_ERROR", "Erro ao conectar com o servidor", t);
                }
            });
    }

    private void handleSuccessfulHours(GetHourResponse body) {
        hourResponseLiveData.setValue(body);
    }

    private void handleHoursError(Response<GetHourResponse> response) {
        String errorBody = "";
        try {
            if (response.errorBody() != null) {
                errorBody = response.errorBody().string();
            }
        } catch (Exception e) {
            errorBody = "Erro ao ler o corpo de erro: " + e.getMessage();
        }
        Log.e("HOUR_FAIL", "Código HTTP: " + response.code() + ", Erro: " + errorBody);
        errorLiveData.setValue("Erro ao consultar: " + errorBody);
    }
}
